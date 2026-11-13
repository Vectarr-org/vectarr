using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Vectarr.Palooza.ApiBusiness.IServices;
using Vectarr.Palooza.ApiBusiness.IServices.IUserServices;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.Classes.BaseApi;
using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.DataAccess.IRepositories;
using Vectarr.Palooza.Exceptions;
using Vectarr.Palooza.Model;

namespace Vectarr.Palooza.ApiBusiness.Services.QuoteRequestServices
{
    /// <summary>
    /// Instant quote requests: creation (with CAD/drawing upload), owner/token/admin authorization,
    /// customer and admin listings, admin updates/stats, downloads and the idempotent "mark paid" step.
    /// Stripe specific logic lives in QuoteRequestPaymentServices.
    /// </summary>
    public class QuoteRequestServices : BaseGetManager<QuoteRequestModel>
    {
        public const long MaxSnapshotBytes = 2L * 1024 * 1024;
        public const int MaxQuantity = 100000;

        public static readonly string[] AllowedCadExtensions = { ".stl", ".step", ".stp", ".3mf", ".dxf", ".ipt", ".sat", ".sldprt" };
        public static readonly string[] AllowedDrawingExtensions = { ".pdf", ".dxf", ".dwg", ".png", ".jpg", ".jpeg" };

        private const int MaxPageSize = 100;
        private const int MaxAdminNotesLength = 20000;
        private const int MaxSearchLength = 200;
        // Uppercase alphanumerics without the easily confused 0/O and 1/I.
        private const string ReferenceAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly JsonSerializerOptions PayloadJsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IImageManager _imageManager;
        private readonly IVectarrAdminService _vectarrAdminService;
        private readonly QuoteRequestNotificationServices _notificationServices;
        private readonly ILogger<QuoteRequestServices> _logger;

        public QuoteRequestServices(IUnitOfWork unitOfWork, IConfiguration configuration, IImageManager imageManager,
            IVectarrAdminService vectarrAdminService, QuoteRequestNotificationServices notificationServices,
            ILogger<QuoteRequestServices> logger) : base(unitOfWork, unitOfWork.Repositories.QuoteRequestRepository)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _imageManager = imageManager;
            _vectarrAdminService = vectarrAdminService;
            _notificationServices = notificationServices;
            _logger = logger;
        }

        private IQuoteRequestRepository Repository => _unitOfWork.Repositories.QuoteRequestRepository;

        private int? CurrentIdentityId => _unitOfWork.CurrentUser != null ? _unitOfWork.Identity?.Id : null;

        /// <summary>
        /// Email used to match guest requests (IdentityId null) to the logged in user: the identity's EmailAddress
        /// (falling back to the account email). Only returned when the account email is CONFIRMED, because
        /// User/Register hands out a JWT before confirmation and nobody must be able to claim someone else's
        /// guest requests by registering with their address.
        /// </summary>
        private string? CurrentOwnerEmail
        {
            get
            {
                var user = _unitOfWork.CurrentUser;
                var identity = _unitOfWork.Identity;
                if (user == null || identity == null || !user.EmailConfirmed)
                {
                    return null;
                }

                var email = string.IsNullOrWhiteSpace(identity.EmailAddress) ? user.Email : identity.EmailAddress;
                return string.IsNullOrWhiteSpace(email) ? null : email.Trim();
            }
        }

        public bool IsCurrentUserVectarrAdmin()
        {
            return _vectarrAdminService.IsCurrentUserVectarrAdmin();
        }

        public InstantQuoteConfig GetInstantQuoteConfig()
        {
            return _configuration.GetSection(InstantQuoteConfig.SectionName).Get<InstantQuoteConfig>() ?? new InstantQuoteConfig();
        }

        #region Create

        public QuoteRequestCreatePayload ParsePayload(string? payloadJson)
        {
            if (string.IsNullOrWhiteSpace(payloadJson))
            {
                throw new ErrorValidationException("The quote payload is required.");
            }

            QuoteRequestCreatePayload? payload;
            try
            {
                payload = JsonSerializer.Deserialize<QuoteRequestCreatePayload>(payloadJson, PayloadJsonOptions);
            }
            catch (JsonException ex)
            {
                throw new ErrorValidationException("The quote payload is not valid JSON: " + ex.Message);
            }

            if (payload == null)
            {
                throw new ErrorValidationException("The quote payload is required.");
            }

            return payload;
        }

        public async Task<QuoteRequestModel> Create(QuoteRequestCreatePayload payload, QuoteRequestUploadFile? cadFile, QuoteRequestUploadFile? drawingFile, string webRootPath)
        {
            var config = GetInstantQuoteConfig();
            var errors = new List<string>();

            // Files
            var cadFileName = ValidateFile(cadFile, AllowedCadExtensions, "CAD file", required: true, errors);
            var drawingFileName = ValidateFile(drawingFile, AllowedDrawingExtensions, "Drawing file", required: false, errors);

            // Contact (defaults from the logged in identity)
            var contactName = Clean(payload.ContactName, 200, "Contact name", errors);
            var contactEmail = Clean(payload.ContactEmail, 256, "Contact email", errors);

            int? identityId = null;
            var identity = _unitOfWork.CurrentUser != null ? _unitOfWork.Identity : null;
            if (identity != null)
            {
                identityId = identity.Id;

                if (string.IsNullOrEmpty(contactName))
                {
                    var fullName = string.Join(" ", new[] { identity.FirstName, identity.LastName }.Where(n => !string.IsNullOrWhiteSpace(n)).Select(n => n!.Trim()));
                    contactName = Truncate(string.IsNullOrWhiteSpace(fullName) ? identity.EmailAddress : fullName, 200);
                }

                if (string.IsNullOrEmpty(contactEmail))
                {
                    contactEmail = Truncate(string.IsNullOrWhiteSpace(identity.EmailAddress) ? _unitOfWork.CurrentUser?.Email : identity.EmailAddress, 256);
                }
            }

            if (string.IsNullOrEmpty(contactName))
            {
                errors.Add("Contact name is required.");
            }

            if (string.IsNullOrEmpty(contactEmail))
            {
                errors.Add("Contact email is required.");
            }
            else if (!EmailRegex.IsMatch(contactEmail))
            {
                errors.Add("Contact email is not a valid email address.");
            }

            // Quantity and prices (computed in the browser: range checks only, an admin reviews before any award)
            if (payload.Quantity < 1 || payload.Quantity > MaxQuantity)
            {
                errors.Add($"Quantity must be between 1 and {MaxQuantity}.");
            }

            var maxTotal = config.MaxTotal > 0 ? config.MaxTotal : 500000m;
            var totalPrice = RoundMoney(payload.TotalPrice);
            var unitPrice = RoundMoney(payload.UnitPrice);

            if (totalPrice <= 0)
            {
                errors.Add("Total price must be greater than 0.");
            }
            else if (totalPrice > maxTotal)
            {
                errors.Add($"Total price cannot exceed {maxTotal.ToString("N2", CultureInfo.InvariantCulture)}. Please contact us for a custom quote.");
            }

            if (unitPrice < 0 || unitPrice > maxTotal)
            {
                errors.Add("Unit price is out of range.");
            }

            if (payload.LeadDays.HasValue && (payload.LeadDays.Value < 0 || payload.LeadDays.Value > 3650))
            {
                errors.Add("Lead days is out of range.");
            }

            if (payload.AssemblyPartCount.HasValue && (payload.AssemblyPartCount.Value < 0 || payload.AssemblyPartCount.Value > MaxQuantity))
            {
                errors.Add("Assembly part count is out of range.");
            }

            ValidateDimension(payload.BboxXmm, "bboxXmm", errors);
            ValidateDimension(payload.BboxYmm, "bboxYmm", errors);
            ValidateDimension(payload.BboxZmm, "bboxZmm", errors);
            ValidateDimension(payload.VolumeMm3, "volumeMm3", errors);

            // Snapshot (string or raw JSON, max 2 MB)
            var snapshot = ReadJsonText(payload.QuoteSnapshotJson);
            if (snapshot != null && Encoding.UTF8.GetByteCount(snapshot) > MaxSnapshotBytes)
            {
                errors.Add("The quote snapshot is too large (max 2 MB).");
            }

            var entity = new QuoteRequest()
            {
                Status = QuoteRequestStatusEnum.AwaitingPayment,
                AccessToken = Guid.NewGuid().ToString("N"),
                IdentityId = identityId,

                ContactName = contactName ?? string.Empty,
                ContactEmail = contactEmail ?? string.Empty,
                Company = Clean(payload.Company, 200, "Company", errors),
                Phone = Clean(payload.Phone, 64, "Phone", errors),

                ShippingStreet1 = Clean(payload.ShippingStreet1, 200, "Shipping street", errors),
                ShippingStreet2 = Clean(payload.ShippingStreet2, 200, "Shipping street (line 2)", errors),
                ShippingCity = Clean(payload.ShippingCity, 100, "Shipping city", errors),
                ShippingState = Clean(payload.ShippingState, 100, "Shipping state", errors),
                ShippingPostalCode = Clean(payload.ShippingPostalCode, 32, "Shipping postal code", errors),
                ShippingCountry = Clean(payload.ShippingCountry, 100, "Shipping country", errors),

                PartName = Clean(payload.PartName, 255, "Part name", errors) ?? cadFileName,
                PartNumber = Clean(payload.PartNumber, 100, "Part number", errors),
                Revision = Clean(payload.Revision, 50, "Revision", errors),
                BuyerNotes = Clean(payload.BuyerNotes, 5000, "Notes", errors),
                SourceFormat = Clean(payload.SourceFormat, 32, "Source format", errors) ?? DefaultSourceFormat(cadFileName),
                IsAssembly = payload.IsAssembly,
                AssemblyPartCount = payload.AssemblyPartCount,

                MaterialId = Clean(payload.MaterialId, 100, "Material id", errors),
                MaterialName = Clean(payload.MaterialName, 200, "Material name", errors),
                ProcessId = Clean(payload.ProcessId, 100, "Process id", errors),
                ProcessName = Clean(payload.ProcessName, 200, "Process name", errors),
                FinishId = Clean(payload.FinishId, 100, "Finish id", errors),
                FinishName = Clean(payload.FinishName, 200, "Finish name", errors),
                ToleranceId = Clean(payload.ToleranceId, 100, "Tolerance id", errors),
                ToleranceLabel = Clean(payload.ToleranceLabel, 200, "Tolerance label", errors),
                SpeedId = Clean(payload.SpeedId, 100, "Speed id", errors),
                SpeedLabel = Clean(payload.SpeedLabel, 200, "Speed label", errors),
                InspectionDocs = Clean(ReadInspectionDocs(payload.InspectionDocs, errors), 500, "Inspection docs", errors),

                Quantity = payload.Quantity,
                UnitPrice = unitPrice,
                TotalPrice = totalPrice,
                AmountDue = ComputeAmountDue(totalPrice, config),
                Currency = GetCurrency(),

                LeadDays = payload.LeadDays,
                ShipByDate = ToUtc(payload.ShipByDate),

                BboxXmm = payload.BboxXmm,
                BboxYmm = payload.BboxYmm,
                BboxZmm = payload.BboxZmm,
                VolumeMm3 = payload.VolumeMm3,

                QuoteSnapshotJson = snapshot,
                CadFileName = cadFileName,
                DrawingFileName = drawingFileName
            };

            if (errors.Any())
            {
                throw new ErrorValidationException(string.Join(Environment.NewLine, errors.Distinct()));
            }

            // Everything is valid: store the files through the existing image pipeline.
            var cadImage = await _imageManager.UploadImage(cadFile!.Content, cadFileName!, webRootPath, Truncate(cadFileName, 100) ?? string.Empty);
            entity.CadFileId = cadImage.Id;

            if (drawingFile != null && drawingFileName != null)
            {
                var drawingImage = await _imageManager.UploadImage(drawingFile.Content, drawingFileName, webRootPath, Truncate(drawingFileName, 100) ?? string.Empty);
                entity.DrawingFileId = drawingImage.Id;
            }

            entity.ReferenceNumber = GenerateUniqueReferenceNumber();
            Repository.Insert(entity);

            _logger.LogInformation("Quote request {ReferenceNumber} ({Id}) created.", entity.ReferenceNumber, entity.Id);

            var model = Repository.GetModel(entity.Id, includeSnapshot: true)
                ?? throw new InvalidOperationException("The quote request could not be loaded after creation.");

            // The creator always gets the access token back.
            return Shape(model, canSeeToken: true, canSeeAdminNotes: IsCurrentUserVectarrAdmin());
        }

        /// <summary>
        /// AmountDue = Round(TotalPrice * InitialFeePercent / 100, 2), at least MinimumCharge.
        /// </summary>
        public static decimal ComputeAmountDue(decimal totalPrice, InstantQuoteConfig config)
        {
            var percent = Math.Clamp(config.InitialFeePercent, 0m, 100m);
            var amount = Math.Round(totalPrice * percent / 100m, 2, MidpointRounding.AwayFromZero);
            var minimum = Math.Round(Math.Max(0m, config.MinimumCharge), 2, MidpointRounding.AwayFromZero);
            return Math.Max(amount, minimum);
        }

        public static string GenerateReferenceNumber(DateTime utcNow)
        {
            var chars = new char[5];
            for (var i = 0; i < chars.Length; i++)
            {
                chars[i] = ReferenceAlphabet[RandomNumberGenerator.GetInt32(ReferenceAlphabet.Length)];
            }

            return "VQ-" + utcNow.ToString("yyMMdd", CultureInfo.InvariantCulture) + "-" + new string(chars);
        }

        private string GenerateUniqueReferenceNumber()
        {
            for (var attempt = 0; attempt < 10; attempt++)
            {
                var candidate = GenerateReferenceNumber(DateTime.UtcNow);
                if (!Repository.ReferenceNumberExists(candidate))
                {
                    return candidate;
                }
            }

            throw new InvalidOperationException("Could not generate a unique quote reference number.");
        }

        private string GetCurrency()
        {
            var currency = _configuration[StripeConfig.SectionName + ":Currency"]?.Trim().ToLowerInvariant();
            return !string.IsNullOrEmpty(currency) && currency.Length == 3 && currency.All(char.IsLetter) ? currency : "usd";
        }

        #endregion

        #region Authorization and shaping

        /// <summary>
        /// Returns the request when the caller may see it (matching access token, owner or Vectarr admin).
        /// Returns null when it does not exist or the caller is not allowed (callers answer 404 in both cases).
        /// A guest request owned through the email rule is linked to the caller's identity on first access.
        /// Used by GetPublic, CreatePaymentIntent, ConfirmPayment and DownloadFile.
        /// </summary>
        public QuoteRequest? GetAuthorizedEntity(int id, string? token)
        {
            if (id <= 0)
            {
                return null;
            }

            var entity = Repository.GetEntityById(id, skipApplicationCheck: true);
            if (entity == null)
            {
                return null;
            }

            var isOwner = IsOwner(entity);
            if (!(isOwner || TokenMatches(entity, token) || IsCurrentUserVectarrAdmin()))
            {
                return null;
            }

            if (isOwner && !entity.IdentityId.HasValue)
            {
                LinkToCurrentIdentity(entity);
            }

            return entity;
        }

        /// <summary>
        /// Loads the full model (with snapshot) for a request already authorized through GetAuthorizedEntity.
        /// </summary>
        public QuoteRequestModel GetModelForCaller(QuoteRequest entity, string? token)
        {
            var model = Repository.GetModel(entity.Id, includeSnapshot: true)
                ?? throw new InvalidOperationException("The quote request could not be loaded.");

            return Shape(model,
                canSeeToken: IsOwner(entity) || TokenMatches(entity, token),
                canSeeAdminNotes: IsCurrentUserVectarrAdmin());
        }

        public QuoteRequestModel? GetPublic(int id, string? token)
        {
            var entity = GetAuthorizedEntity(id, token);
            return entity == null ? null : GetModelForCaller(entity, token);
        }

        /// <summary>
        /// Owner = IdentityId == current identity id, OR (IdentityId is null AND ContactEmail equals the current
        /// identity's confirmed email, case-insensitive) so guests who check out first and register later see their requests.
        /// </summary>
        private bool IsOwner(QuoteRequest entity)
        {
            var identityId = CurrentIdentityId;
            if (!identityId.HasValue)
            {
                return false;
            }

            if (entity.IdentityId == identityId.Value)
            {
                return true;
            }

            var ownerEmail = CurrentOwnerEmail;
            return !entity.IdentityId.HasValue
                && ownerEmail != null
                && string.Equals(entity.ContactEmail?.Trim(), ownerEmail, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Persists the guest -> account link. Failures are logged only (access is already granted by the email rule).
        /// </summary>
        private void LinkToCurrentIdentity(QuoteRequest entity)
        {
            var identityId = CurrentIdentityId;
            if (!identityId.HasValue)
            {
                return;
            }

            try
            {
                entity.IdentityId = identityId.Value;
                Repository.Update(entity);
                _logger.LogInformation("Guest quote request {Id} linked to identity {IdentityId}.", entity.Id, identityId.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not link guest quote request {Id} to identity {IdentityId}.", entity.Id, identityId.Value);
            }
        }

        private static bool TokenMatches(QuoteRequest entity, string? token)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrEmpty(entity.AccessToken))
            {
                return false;
            }

            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(entity.AccessToken),
                Encoding.UTF8.GetBytes(token.Trim()));
        }

        private static QuoteRequestModel Shape(QuoteRequestModel model, bool canSeeToken, bool canSeeAdminNotes)
        {
            if (!canSeeToken)
            {
                model.AccessToken = null;
            }

            if (!canSeeAdminNotes)
            {
                model.AdminNotes = null;
            }

            return model;
        }

        #endregion

        #region Lists

        public ListViewModel<QuoteRequestModel> GetMine(int pageNumber, int countPerPage)
        {
            NormalizePaging(ref pageNumber, ref countPerPage, 10);

            var identityId = CurrentIdentityId;
            if (!identityId.HasValue)
            {
                return new ListViewModel<QuoteRequestModel>(new List<QuoteRequestModel>(), 0);
            }

            // Guest requests checked out with this (confirmed) email before the account existed are owned too;
            // link them to the identity on first listing so the relation is persisted.
            var ownerEmail = CurrentOwnerEmail;
            if (ownerEmail != null)
            {
                try
                {
                    var linked = Repository.LinkGuestRequests(identityId.Value, ownerEmail);
                    if (linked > 0)
                    {
                        _logger.LogInformation("Linked {Count} guest quote request(s) to identity {IdentityId}.", linked, identityId.Value);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Could not link guest quote requests to identity {IdentityId}.", identityId.Value);
                }
            }

            var data = Repository.Search(identityId, ownerEmail, null, null, pageNumber, countPerPage, out var totalCount);
            var isAdmin = IsCurrentUserVectarrAdmin();
            data.ForEach(m => Shape(m, canSeeToken: true, canSeeAdminNotes: isAdmin));

            return new ListViewModel<QuoteRequestModel>(data, totalCount);
        }

        /// <summary>Money Center totals of the caller's own requests (empty for guests).</summary>
        public QuoteRequestPaymentSummaryModel GetMinePaymentSummary()
        {
            var identityId = CurrentIdentityId;
            if (!identityId.HasValue)
            {
                return new QuoteRequestPaymentSummaryModel();
            }

            return Repository.GetPaymentSummary(identityId.Value, CurrentOwnerEmail);
        }

        /// <summary>Vectarr admin only (checked by the controller).</summary>
        public ListViewModel<QuoteRequestModel> AdminList(int pageNumber, int countPerPage, int? status, string? search)
        {
            NormalizePaging(ref pageNumber, ref countPerPage, 20);

            QuoteRequestStatusEnum? statusFilter = null;
            if (status.HasValue)
            {
                if (!Enum.IsDefined(typeof(QuoteRequestStatusEnum), status.Value))
                {
                    throw new ErrorValidationException("Unknown quote request status.");
                }
                statusFilter = (QuoteRequestStatusEnum)status.Value;
            }

            var term = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
            if (term != null && term.Length > MaxSearchLength)
            {
                term = term.Substring(0, MaxSearchLength);
            }

            var data = Repository.Search(null, null, statusFilter, term, pageNumber, countPerPage, out var totalCount);
            var identityId = CurrentIdentityId;
            data.ForEach(m => Shape(m, canSeeToken: identityId.HasValue && m.IdentityId == identityId.Value, canSeeAdminNotes: true));

            return new ListViewModel<QuoteRequestModel>(data, totalCount);
        }

        private static void NormalizePaging(ref int pageNumber, ref int countPerPage, int defaultCountPerPage)
        {
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            if (countPerPage < 1)
            {
                countPerPage = defaultCountPerPage;
            }
            else if (countPerPage > MaxPageSize)
            {
                countPerPage = MaxPageSize;
            }
        }

        #endregion

        #region Admin

        /// <summary>Vectarr admin only (checked by the controller).</summary>
        public QuoteRequestModel? AdminGet(int id)
        {
            var model = Repository.GetModel(id, includeSnapshot: true);
            if (model == null)
            {
                return null;
            }

            var identityId = CurrentIdentityId;
            return Shape(model, canSeeToken: identityId.HasValue && model.IdentityId == identityId.Value, canSeeAdminNotes: true);
        }

        /// <summary>Vectarr admin only (checked by the controller). Null fields are left unchanged.</summary>
        public QuoteRequestModel? AdminUpdate(QuoteRequestAdminUpdateModel update)
        {
            var entity = Repository.GetEntityById(update.Id, skipApplicationCheck: true);
            if (entity == null)
            {
                return null;
            }

            if (update.Status.HasValue)
            {
                if (!Enum.IsDefined(typeof(QuoteRequestStatusEnum), update.Status.Value))
                {
                    throw new ErrorValidationException("Unknown quote request status.");
                }
                entity.Status = (QuoteRequestStatusEnum)update.Status.Value;
            }

            if (update.AdminNotes != null)
            {
                if (update.AdminNotes.Length > MaxAdminNotesLength)
                {
                    throw new ErrorValidationException($"Admin notes are too long (max {MaxAdminNotesLength} characters).");
                }
                entity.AdminNotes = string.IsNullOrWhiteSpace(update.AdminNotes) ? null : update.AdminNotes;
            }

            Repository.Update(entity);

            return AdminGet(entity.Id);
        }

        /// <summary>Vectarr admin only (checked by the controller).</summary>
        public QuoteRequestStatsModel AdminStats()
        {
            return Repository.GetStats();
        }

        #endregion

        #region Files

        /// <summary>
        /// Resolves a stored CAD/drawing file for download. Null when not found or not allowed.
        /// </summary>
        public QuoteRequestDownload? GetDownload(int id, string? kind, string? token)
        {
            var normalizedKind = kind?.Trim().ToLowerInvariant();
            if (normalizedKind != "cad" && normalizedKind != "drawing")
            {
                throw new ErrorValidationException("kind must be 'cad' or 'drawing'.");
            }

            var entity = GetAuthorizedEntity(id, token);
            if (entity == null)
            {
                return null;
            }

            var isCad = normalizedKind == "cad";
            var imageId = isCad ? entity.CadFileId : entity.DrawingFileId;
            if (!imageId.HasValue)
            {
                return null;
            }

            var image = _unitOfWork.Repositories.ImageRepository.GetEntityById(imageId.Value, skipApplicationCheck: true);
            var basePath = _configuration["FileStorage:BasePath"];
            if (image == null || string.IsNullOrWhiteSpace(image.OriginalPath) || string.IsNullOrWhiteSpace(basePath))
            {
                return null;
            }

            // LocalStorageManager stores files flat under FileStorage:BasePath; Image.OriginalPath is the stored file name.
            var storedName = Path.GetFileName(image.OriginalPath);
            var physicalPath = Path.GetFullPath(Path.Combine(basePath, storedName));
            if (!System.IO.File.Exists(physicalPath))
            {
                _logger.LogWarning("Quote request {Id} {Kind} file is missing on disk.", entity.Id, normalizedKind);
                return null;
            }

            var downloadName = (isCad ? entity.CadFileName : entity.DrawingFileName);
            if (string.IsNullOrWhiteSpace(downloadName))
            {
                downloadName = storedName;
            }

            return new QuoteRequestDownload(physicalPath, downloadName, GetContentType(downloadName));
        }

        private static string GetContentType(string fileName)
        {
            switch (Path.GetExtension(fileName).ToLowerInvariant())
            {
                case ".pdf":
                    return "application/pdf";
                case ".png":
                    return "image/png";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".stl":
                    return "model/stl";
                case ".step":
                case ".stp":
                    return "model/step";
                case ".3mf":
                    return "model/3mf";
                case ".dxf":
                    return "image/vnd.dxf";
                case ".dwg":
                    return "image/vnd.dwg";
                default:
                    return "application/octet-stream";
            }
        }

        #endregion

        #region Payment

        /// <summary>
        /// Records a successful payment exactly once (atomic conditional update) and, for the call that
        /// performed the update, sends the admin + customer notifications. Notification failures are logged only.
        /// </summary>
        public bool MarkPaid(int id, decimal amountPaid, string? paymentIntentId, string? checkoutSessionId)
        {
            var updated = Repository.TryMarkPaid(id, RoundMoney(amountPaid), paymentIntentId, checkoutSessionId, DateTime.UtcNow);
            if (!updated)
            {
                return false;
            }

            _logger.LogInformation("Quote request {Id} marked as paid ({AmountPaid}).", id, amountPaid);

            try
            {
                var model = Repository.GetModel(id, includeSnapshot: false);
                if (model != null)
                {
                    _notificationServices.SendPaidNotifications(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not send the paid notifications for quote request {Id}.", id);
            }

            return true;
        }

        #endregion

        #region Helpers

        private static decimal RoundMoney(decimal value)
        {
            return Math.Round(value, 2, MidpointRounding.AwayFromZero);
        }

        private static string? Clean(string? value, int maxLength, string label, List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var trimmed = value.Trim();
            if (trimmed.Length > maxLength)
            {
                errors.Add($"{label} is too long (max {maxLength} characters).");
            }

            return trimmed;
        }

        private static string? Truncate(string? value, int maxLength)
        {
            if (value == null)
            {
                return null;
            }

            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        private static void ValidateDimension(double? value, string label, List<string> errors)
        {
            if (value.HasValue && (!double.IsFinite(value.Value) || value.Value < 0))
            {
                errors.Add($"{label} must be a positive number.");
            }
        }

        private static DateTime? ToUtc(DateTime? value)
        {
            if (!value.HasValue)
            {
                return null;
            }

            switch (value.Value.Kind)
            {
                case DateTimeKind.Utc:
                    return value.Value;
                case DateTimeKind.Local:
                    return value.Value.ToUniversalTime();
                default:
                    return DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
            }
        }

        private static string? ReadJsonText(JsonElement? element)
        {
            if (!element.HasValue)
            {
                return null;
            }

            switch (element.Value.ValueKind)
            {
                case JsonValueKind.Undefined:
                case JsonValueKind.Null:
                    return null;
                case JsonValueKind.String:
                    var text = element.Value.GetString();
                    return string.IsNullOrWhiteSpace(text) ? null : text;
                default:
                    return element.Value.GetRawText();
            }
        }

        private static string? ReadInspectionDocs(JsonElement? element, List<string> errors)
        {
            if (!element.HasValue)
            {
                return null;
            }

            switch (element.Value.ValueKind)
            {
                case JsonValueKind.Undefined:
                case JsonValueKind.Null:
                    return null;
                case JsonValueKind.String:
                    return element.Value.GetString();
                case JsonValueKind.Array:
                    var ids = new List<string>();
                    foreach (var item in element.Value.EnumerateArray())
                    {
                        if (item.ValueKind != JsonValueKind.String)
                        {
                            errors.Add("inspectionDocs must be a comma separated string or an array of strings.");
                            return null;
                        }

                        var id = item.GetString();
                        if (!string.IsNullOrWhiteSpace(id))
                        {
                            ids.Add(id.Trim());
                        }
                    }
                    return string.Join(",", ids);
                default:
                    errors.Add("inspectionDocs must be a comma separated string or an array of strings.");
                    return null;
            }
        }

        private static string? ValidateFile(QuoteRequestUploadFile? file, string[] allowedExtensions, string label, bool required, List<string> errors)
        {
            if (file == null || (file.Length == 0 && string.IsNullOrWhiteSpace(file.FileName)))
            {
                if (required)
                {
                    errors.Add($"{label} is required.");
                }
                return null;
            }

            var fileName = SanitizeFileName(file.FileName);
            if (string.IsNullOrEmpty(fileName))
            {
                errors.Add($"{label} must have a file name.");
                return null;
            }

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                errors.Add($"{label} type '{extension}' is not supported. Allowed: {string.Join(" ", allowedExtensions)}.");
                return null;
            }

            if (file.Length <= 0)
            {
                errors.Add($"{label} is empty.");
                return null;
            }

            return fileName;
        }

        private static string SanitizeFileName(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return string.Empty;
            }

            // Keep only the last path segment (some browsers send full paths) and drop control characters/quotes.
            var name = fileName.Replace('\\', '/');
            var lastSlash = name.LastIndexOf('/');
            if (lastSlash >= 0)
            {
                name = name.Substring(lastSlash + 1);
            }

            name = new string(name.Where(c => !char.IsControl(c) && c != '"').ToArray()).Trim();

            if (name.Length > 255)
            {
                var extension = Path.GetExtension(name);
                if (extension.Length > 20)
                {
                    extension = string.Empty;
                }
                name = name.Substring(0, 255 - extension.Length) + extension;
            }

            return name;
        }

        private static string? DefaultSourceFormat(string? cadFileName)
        {
            if (string.IsNullOrEmpty(cadFileName))
            {
                return null;
            }

            var extension = Path.GetExtension(cadFileName).TrimStart('.').ToUpperInvariant();
            return extension == "STP" ? "STEP" : (string.IsNullOrEmpty(extension) ? null : extension);
        }

        #endregion
    }
}
