using System.Linq.Expressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.DataAccess.Infrastructure;
using Vectarr.Palooza.DataAccess.IRepositories;
using Vectarr.Palooza.Model;

namespace Vectarr.Palooza.DataAccess.Repositories
{
    public class QuoteRequestRepository : BaseRepository<QuoteRequest, QuoteRequestModel>, IQuoteRequestRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public QuoteRequestRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Plain EF queryable (no LinqKit wrapper) for aggregates and bulk updates.
        /// </summary>
        private IQueryable<QuoteRequest> PlainQuery(bool includeRemoved = false)
        {
            return _unitOfWork.GetGenericRepository<QuoteRequest>().GetQueryable(includeRemoved);
        }

        public override Expression<Func<QuoteRequest, QuoteRequestModel>> GetEntityToModelExpression()
        {
            // QuoteSnapshotJson is intentionally not projected here (it can be up to 2 MB); see GetModel.
            return entity => new QuoteRequestModel()
            {
                Id = entity.Id,
                DateCreated = entity.CreatedDate,
                LastModified = entity.LastModified,
                ReferenceNumber = entity.ReferenceNumber,
                Status = entity.Status,
                AccessToken = entity.AccessToken,
                IdentityId = entity.IdentityId,
                JobId = entity.JobId,

                ContactName = entity.ContactName,
                ContactEmail = entity.ContactEmail,
                Company = entity.Company,
                Phone = entity.Phone,

                ShippingStreet1 = entity.ShippingStreet1,
                ShippingStreet2 = entity.ShippingStreet2,
                ShippingCity = entity.ShippingCity,
                ShippingState = entity.ShippingState,
                ShippingPostalCode = entity.ShippingPostalCode,
                ShippingCountry = entity.ShippingCountry,

                PartName = entity.PartName,
                PartNumber = entity.PartNumber,
                Revision = entity.Revision,
                BuyerNotes = entity.BuyerNotes,
                SourceFormat = entity.SourceFormat,
                IsAssembly = entity.IsAssembly,
                AssemblyPartCount = entity.AssemblyPartCount,

                MaterialId = entity.MaterialId,
                MaterialName = entity.MaterialName,
                ProcessId = entity.ProcessId,
                ProcessName = entity.ProcessName,
                FinishId = entity.FinishId,
                FinishName = entity.FinishName,
                ToleranceId = entity.ToleranceId,
                ToleranceLabel = entity.ToleranceLabel,
                SpeedId = entity.SpeedId,
                SpeedLabel = entity.SpeedLabel,
                InspectionDocs = entity.InspectionDocs,

                Quantity = entity.Quantity,
                UnitPrice = entity.UnitPrice,
                TotalPrice = entity.TotalPrice,
                AmountDue = entity.AmountDue,
                AmountPaid = entity.AmountPaid,
                Currency = entity.Currency,

                LeadDays = entity.LeadDays,
                ShipByDate = entity.ShipByDate,

                BboxXmm = entity.BboxXmm,
                BboxYmm = entity.BboxYmm,
                BboxZmm = entity.BboxZmm,
                VolumeMm3 = entity.VolumeMm3,

                CadFileId = entity.CadFileId,
                CadFile = entity.CadFile != null ? new QuoteRequestFileModel()
                {
                    Id = entity.CadFile.Id,
                    DateCreated = entity.CadFile.CreatedDate,
                    LastModified = entity.CadFile.LastModified,
                    RelativeOriginalPath = entity.CadFile.OriginalPath,
                    RelativeThumbnailOriginalPath = entity.CadFile.RelativeThumbnailOriginalPath,
                    BaseStorageUrl = entity.CadFile.StorageUrl,
                    AltText = entity.CadFile.AltText,
                    FileName = entity.CadFileName
                } : null,
                DrawingFileId = entity.DrawingFileId,
                DrawingFile = entity.DrawingFile != null ? new QuoteRequestFileModel()
                {
                    Id = entity.DrawingFile.Id,
                    DateCreated = entity.DrawingFile.CreatedDate,
                    LastModified = entity.DrawingFile.LastModified,
                    RelativeOriginalPath = entity.DrawingFile.OriginalPath,
                    RelativeThumbnailOriginalPath = entity.DrawingFile.RelativeThumbnailOriginalPath,
                    BaseStorageUrl = entity.DrawingFile.StorageUrl,
                    AltText = entity.DrawingFile.AltText,
                    FileName = entity.DrawingFileName
                } : null,

                StripeCheckoutSessionId = entity.StripeCheckoutSessionId,
                StripePaymentIntentId = entity.StripePaymentIntentId,
                PaidAt = entity.PaidAt,

                AdminNotes = entity.AdminNotes
            };
        }

        /// <summary>
        /// Only the descriptive, client-provided fields are mapped here. Server controlled fields
        /// (reference, token, status, identity, amounts due/paid, currency, files, Stripe ids, admin notes)
        /// are set explicitly by QuoteRequestServices.
        /// </summary>
        protected override QuoteRequest ModelToEntityInternal(QuoteRequestModel model, QuoteRequest entity)
        {
            entity.ContactName = model.ContactName;
            entity.ContactEmail = model.ContactEmail;
            entity.Company = model.Company;
            entity.Phone = model.Phone;

            entity.ShippingStreet1 = model.ShippingStreet1;
            entity.ShippingStreet2 = model.ShippingStreet2;
            entity.ShippingCity = model.ShippingCity;
            entity.ShippingState = model.ShippingState;
            entity.ShippingPostalCode = model.ShippingPostalCode;
            entity.ShippingCountry = model.ShippingCountry;

            entity.PartName = model.PartName;
            entity.PartNumber = model.PartNumber;
            entity.Revision = model.Revision;
            entity.BuyerNotes = model.BuyerNotes;
            entity.SourceFormat = model.SourceFormat;
            entity.IsAssembly = model.IsAssembly;
            entity.AssemblyPartCount = model.AssemblyPartCount;

            entity.MaterialId = model.MaterialId;
            entity.MaterialName = model.MaterialName;
            entity.ProcessId = model.ProcessId;
            entity.ProcessName = model.ProcessName;
            entity.FinishId = model.FinishId;
            entity.FinishName = model.FinishName;
            entity.ToleranceId = model.ToleranceId;
            entity.ToleranceLabel = model.ToleranceLabel;
            entity.SpeedId = model.SpeedId;
            entity.SpeedLabel = model.SpeedLabel;
            entity.InspectionDocs = model.InspectionDocs;

            entity.Quantity = model.Quantity;
            entity.LeadDays = model.LeadDays;
            entity.ShipByDate = model.ShipByDate;

            entity.BboxXmm = model.BboxXmm;
            entity.BboxYmm = model.BboxYmm;
            entity.BboxZmm = model.BboxZmm;
            entity.VolumeMm3 = model.VolumeMm3;

            return entity;
        }

        public QuoteRequestModel? GetModel(int id, bool includeSnapshot)
        {
            var model = QueryItem(x => x.Id == id, skipApplicationCheck: true);

            if (model != null && includeSnapshot)
            {
                model.QuoteSnapshotJson = PlainQuery()
                    .Where(x => x.Id == id)
                    .Select(x => x.QuoteSnapshotJson)
                    .FirstOrDefault();
            }

            return model;
        }

        public List<QuoteRequestModel> Search(int? ownerIdentityId, string? ownerEmail, QuoteRequestStatusEnum? status, string? search, int pageNumber, int countPerPage, out int totalCount)
        {
            if (pageNumber < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageNumber), "The page number should be 1 or higher");
            }

            var query = ApplyOwnerFilter(GetEntitiesQueryable(skipApplicationCheck: true), ownerIdentityId, ownerEmail);

            if (status.HasValue)
            {
                var statusValue = status.Value;
                query = query.Where(x => x.Status == statusValue);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLowerInvariant();
                query = query.Where(x =>
                    x.ReferenceNumber.ToLower().Contains(term) ||
                    x.ContactName.ToLower().Contains(term) ||
                    x.ContactEmail.ToLower().Contains(term) ||
                    (x.Company != null && x.Company.ToLower().Contains(term)) ||
                    (x.PartName != null && x.PartName.ToLower().Contains(term)) ||
                    (x.PartNumber != null && x.PartNumber.ToLower().Contains(term)));
            }

            totalCount = query.Count();

            return query
                .OrderByDescending(x => x.CreatedDate)
                .ThenByDescending(x => x.Id)
                .Skip((pageNumber - 1) * countPerPage)
                .Take(countPerPage)
                .Select(GetEntityToModelExpression().Expand())
                .ToList();
        }

        public int LinkGuestRequests(int identityId, string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return 0;
            }

            var normalizedEmail = email.Trim().ToLowerInvariant();
            var now = DateTime.UtcNow;
            int? newIdentityId = identityId;

            return PlainQuery()
                .Where(x => x.IdentityId == null && x.ContactEmail.ToLower() == normalizedEmail)
                .ExecuteUpdate(setters => setters
                    .SetProperty(x => x.IdentityId, newIdentityId)
                    .SetProperty(x => x.LastModified, now));
        }

        /// <summary>
        /// Owned = IdentityId == ownerIdentityId OR (IdentityId is null AND ContactEmail == ownerEmail, case-insensitive).
        /// No filter when ownerIdentityId is null.
        /// </summary>
        private static IQueryable<QuoteRequest> ApplyOwnerFilter(IQueryable<QuoteRequest> query, int? ownerIdentityId, string? ownerEmail)
        {
            if (!ownerIdentityId.HasValue)
            {
                return query;
            }

            var id = ownerIdentityId.Value;
            if (string.IsNullOrWhiteSpace(ownerEmail))
            {
                return query.Where(x => x.IdentityId == id);
            }

            var normalizedEmail = ownerEmail.Trim().ToLowerInvariant();
            return query.Where(x => x.IdentityId == id || (x.IdentityId == null && x.ContactEmail.ToLower() == normalizedEmail));
        }

        public bool ReferenceNumberExists(string referenceNumber)
        {
            // Include removed rows: the unique index covers them too.
            return PlainQuery(includeRemoved: true).Any(x => x.ReferenceNumber == referenceNumber);
        }

        public bool TryMarkPaid(int id, decimal amountPaid, string? paymentIntentId, string? checkoutSessionId, DateTime paidAtUtc)
        {
            var now = DateTime.UtcNow;

            // Single conditional UPDATE so concurrent confirmations (success page + webhook) cannot both win.
            // Requests still awaiting payment (or cancelled before the money arrived) become Paid;
            // any status an admin already moved forward is kept, but the payment is still recorded.
            var affected = PlainQuery()
                .Where(x => x.Id == id && x.PaidAt == null)
                .ExecuteUpdate(setters => setters
                    .SetProperty(x => x.Status, x => x.Status == QuoteRequestStatusEnum.AwaitingPayment || x.Status == QuoteRequestStatusEnum.Cancelled
                        ? QuoteRequestStatusEnum.Paid
                        : x.Status)
                    .SetProperty(x => x.AmountPaid, amountPaid)
                    .SetProperty(x => x.PaidAt, paidAtUtc)
                    .SetProperty(x => x.StripePaymentIntentId, x => paymentIntentId ?? x.StripePaymentIntentId)
                    .SetProperty(x => x.StripeCheckoutSessionId, x => checkoutSessionId ?? x.StripeCheckoutSessionId)
                    .SetProperty(x => x.LastModified, now));

            return affected > 0;
        }

        public QuoteRequestStatsModel GetStats()
        {
            var counts = PlainQuery()
                .GroupBy(x => x.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToList()
                .ToDictionary(x => x.Status, x => x.Count);

            var paidAmount = PlainQuery()
                .Where(x => x.AmountPaid != null && x.Status != QuoteRequestStatusEnum.Refunded)
                .Sum(x => x.AmountPaid) ?? 0m;

            int CountOf(QuoteRequestStatusEnum status) => counts.TryGetValue(status, out var count) ? count : 0;

            return new QuoteRequestStatsModel()
            {
                Total = counts.Values.Sum(),
                AwaitingPayment = CountOf(QuoteRequestStatusEnum.AwaitingPayment),
                Paid = CountOf(QuoteRequestStatusEnum.Paid),
                InReview = CountOf(QuoteRequestStatusEnum.InReview),
                SentToWorkshops = CountOf(QuoteRequestStatusEnum.SentToWorkshops),
                Completed = CountOf(QuoteRequestStatusEnum.Completed),
                Cancelled = CountOf(QuoteRequestStatusEnum.Cancelled),
                Refunded = CountOf(QuoteRequestStatusEnum.Refunded),
                PaidAmount = paidAmount
            };
        }

        public QuoteRequestPaymentSummaryModel GetPaymentSummary(int ownerIdentityId, string? ownerEmail)
        {
            var owned = ApplyOwnerFilter(PlainQuery(), ownerIdentityId, ownerEmail);

            var paid = owned.Where(x => x.AmountPaid != null && x.Status != QuoteRequestStatusEnum.Refunded);
            var awaiting = owned.Where(x => x.Status == QuoteRequestStatusEnum.AwaitingPayment);

            return new QuoteRequestPaymentSummaryModel()
            {
                TotalPaid = paid.Sum(x => x.AmountPaid) ?? 0m,
                PaidCount = paid.Count(),
                AwaitingPaymentAmount = awaiting.Sum(x => (decimal?)x.AmountDue) ?? 0m,
                AwaitingPaymentCount = awaiting.Count(),
                LastPaidAt = paid.Max(x => x.PaidAt)
            };
        }
    }
}
