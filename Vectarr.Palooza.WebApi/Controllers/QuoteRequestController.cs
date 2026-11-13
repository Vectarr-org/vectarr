using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vectarr.Palooza.ApiBusiness.Services.QuoteRequestServices;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.Classes.BaseApi;
using Vectarr.Palooza.DataAccess;

namespace Vectarr.Palooza.WebApi.Controllers
{
    /// <summary>
    /// Instant quote requests (in-house Instant Quote tool + Stripe Payment Element).
    /// Guests authorize with the request's access token, logged in owners with their identity,
    /// and Vectarr admins (Identity.IsVectarrAdmin or VectarrAdmin:Emails) can see everything.
    /// Unknown ids and wrong tokens both answer 404; admin endpoints answer 403 for non-admins.
    /// </summary>
    [Route("api/[controller]")]
    public class QuoteRequestController : SystemController
    {
        /// <summary>150 MB: CAD + optional drawing + payload.</summary>
        public const long MaxUploadBytes = 150L * 1024 * 1024;

        /// <summary>Max size of a single form value (the JSON payload incl. a 2 MB snapshot, escaped).</summary>
        private const int MaxFormValueBytes = 8 * 1024 * 1024;

        private const string AdminOnlyMessage = "This action requires a Vectarr admin account.";

        private readonly QuoteRequestServices _quoteRequestServices;
        private readonly QuoteRequestPaymentServices _paymentServices;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public QuoteRequestController(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor,
            QuoteRequestServices quoteRequestServices, QuoteRequestPaymentServices paymentServices,
            IWebHostEnvironment hostingEnvironment) : base(unitOfWork, httpContextAccessor)
        {
            _quoteRequestServices = quoteRequestServices;
            _paymentServices = paymentServices;
            _hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// multipart/form-data: payload (JSON string), cadFile (required), drawingFile (optional).
        /// </summary>
        [HttpPost]
        [Route("Create")]
        [AllowAnonymous]
        [RequestSizeLimit(MaxUploadBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxUploadBytes, ValueLengthLimit = MaxFormValueBytes)]
        public async Task<ActionResult<QuoteRequestModel>> Create([FromForm] string? payload, IFormFile? cadFile, IFormFile? drawingFile)
        {
            payload ??= await ReadPayloadFromFilePart();
            var parsedPayload = _quoteRequestServices.ParsePayload(payload);

            await using var cadStream = cadFile?.OpenReadStream();
            await using var drawingStream = drawingFile?.OpenReadStream();

            var cad = cadFile != null && cadStream != null
                ? new QuoteRequestUploadFile(cadFile.FileName, cadFile.Length, cadStream)
                : null;
            var drawing = drawingFile != null && drawingStream != null
                ? new QuoteRequestUploadFile(drawingFile.FileName, drawingFile.Length, drawingStream)
                : null;

            return await _quoteRequestServices.Create(parsedPayload, cad, drawing, _hostingEnvironment.WebRootPath ?? string.Empty);
        }

        /// <summary>
        /// Returns the PaymentIntent client secret (+ publishable key) the checkout page's Payment Element pays with.
        /// </summary>
        [HttpPost]
        [Route("CreatePaymentIntent")]
        [AllowAnonymous]
        public async Task<ActionResult<QuoteRequestPaymentIntentModel>> CreatePaymentIntent([FromQuery] int id, [FromQuery] string? token)
        {
            var result = await _paymentServices.CreatePaymentIntent(id, token);
            if (result == null)
            {
                return NotFound();
            }
            return result;
        }

        /// <summary>
        /// Called by the success page after paying; marks the request paid when Stripe says so.
        /// paymentIntentId for Payment Element payments, sessionId for orders paid on the former Stripe Checkout page.
        /// </summary>
        [HttpGet]
        [Route("ConfirmPayment")]
        [AllowAnonymous]
        public async Task<ActionResult<QuoteRequestModel>> ConfirmPayment([FromQuery] int id, [FromQuery] string? token, [FromQuery] string? paymentIntentId, [FromQuery] string? sessionId)
        {
            var result = await _paymentServices.ConfirmPayment(id, token, paymentIntentId, sessionId);
            if (result == null)
            {
                return NotFound();
            }
            return result;
        }

        /// <summary>
        /// Stripe webhook endpoint (configure in Stripe: payment_intent.succeeded; checkout.session.completed and
        /// checkout.session.async_payment_succeeded are still handled for older orders).
        /// </summary>
        [HttpPost]
        [Route("StripeWebhook")]
        [AllowAnonymous]
        public async Task<IActionResult> StripeWebhook()
        {
            string json;
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                json = await reader.ReadToEndAsync();
            }

            var signature = Request.Headers["Stripe-Signature"].ToString();
            var accepted = _paymentServices.HandleWebhook(json, signature);
            return accepted ? Ok() : BadRequest("Invalid Stripe webhook signature.");
        }

        [HttpGet]
        [Route("GetPublic")]
        [AllowAnonymous]
        public ActionResult<QuoteRequestModel> GetPublic([FromQuery] int id, [FromQuery] string? token)
        {
            var result = _quoteRequestServices.GetPublic(id, token);
            if (result == null)
            {
                return NotFound();
            }
            return result;
        }

        [HttpGet]
        [Route("Mine")]
        [Authorize]
        public ActionResult<ListViewModel<QuoteRequestModel>> Mine([FromQuery] int pageNumber = 1, [FromQuery] int countPerPage = 10)
        {
            return _quoteRequestServices.GetMine(pageNumber, countPerPage);
        }

        [HttpGet]
        [Route("MinePaymentSummary")]
        [Authorize]
        public ActionResult<QuoteRequestPaymentSummaryModel> MinePaymentSummary()
        {
            return _quoteRequestServices.GetMinePaymentSummary();
        }

        [HttpGet]
        [Route("AdminList")]
        public ActionResult<ListViewModel<QuoteRequestModel>> AdminList([FromQuery] int pageNumber = 1, [FromQuery] int countPerPage = 20, [FromQuery] int? status = null, [FromQuery] string? search = null)
        {
            if (!_quoteRequestServices.IsCurrentUserVectarrAdmin())
            {
                return AdminForbidden();
            }

            return _quoteRequestServices.AdminList(pageNumber, countPerPage, status, search);
        }

        [HttpGet]
        [Route("AdminGet")]
        public ActionResult<QuoteRequestModel> AdminGet([FromQuery] int id)
        {
            if (!_quoteRequestServices.IsCurrentUserVectarrAdmin())
            {
                return AdminForbidden();
            }

            var result = _quoteRequestServices.AdminGet(id);
            if (result == null)
            {
                return NotFound();
            }
            return result;
        }

        [HttpPost]
        [Route("AdminUpdate")]
        public ActionResult<QuoteRequestModel> AdminUpdate([FromBody] QuoteRequestAdminUpdateModel model)
        {
            if (!_quoteRequestServices.IsCurrentUserVectarrAdmin())
            {
                return AdminForbidden();
            }

            var result = _quoteRequestServices.AdminUpdate(model);
            if (result == null)
            {
                return NotFound();
            }
            return result;
        }

        [HttpGet]
        [Route("AdminStats")]
        public ActionResult<QuoteRequestStatsModel> AdminStats()
        {
            if (!_quoteRequestServices.IsCurrentUserVectarrAdmin())
            {
                return AdminForbidden();
            }

            return _quoteRequestServices.AdminStats();
        }

        /// <summary>
        /// Streams the stored CAD (kind=cad) or drawing (kind=drawing) file as an attachment with its original name.
        /// </summary>
        [HttpGet]
        [Route("DownloadFile")]
        [AllowAnonymous]
        public IActionResult DownloadFile([FromQuery] int id, [FromQuery] string? kind, [FromQuery] string? token)
        {
            var download = _quoteRequestServices.GetDownload(id, kind, token);
            if (download == null)
            {
                return NotFound();
            }

            return PhysicalFile(download.PhysicalPath, download.ContentType, download.FileName, enableRangeProcessing: true);
        }

        private ObjectResult AdminForbidden()
        {
            return StatusCode(StatusCodes.Status403Forbidden, AdminOnlyMessage);
        }

        /// <summary>
        /// Lenient fallback: accept the payload sent as a file/blob part named "payload".
        /// </summary>
        private async Task<string?> ReadPayloadFromFilePart()
        {
            if (!Request.HasFormContentType)
            {
                return null;
            }

            var part = Request.Form.Files.GetFile("payload");
            if (part == null || part.Length == 0 || part.Length > MaxFormValueBytes)
            {
                return null;
            }

            using var reader = new StreamReader(part.OpenReadStream(), Encoding.UTF8);
            return await reader.ReadToEndAsync();
        }
    }
}
