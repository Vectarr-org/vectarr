using System.Security.Cryptography;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vectarr.Palooza.ApiBusiness.IServices.IUserServices;
using Vectarr.Palooza.ApiBusiness.Services.Metrics;
using Vectarr.Palooza.Classes.Metrics;
using Vectarr.Palooza.DataAccess;

namespace Vectarr.Palooza.WebApi.Controllers
{
    [ApiController]
    [Route("api/metrics")]
    public class MetricsController : ControllerBase
    {
        public const string ReadKeyHeader = "X-Metrics-Key";
        public const string ReadKeyConfig = "SiteFunnel:ReadKey";

        private readonly SiteFunnelService _funnel;
        private readonly IConfiguration _configuration;
        private readonly IVectarrAdminService _admin;
        private readonly IUnitOfWork _unitOfWork;

        public MetricsController(
            SiteFunnelService funnel,
            IConfiguration configuration,
            IVectarrAdminService admin,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor)
        {
            _funnel = funnel;
            _configuration = configuration;
            _admin = admin;
            _unitOfWork = unitOfWork;

            if (httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true)
            {
                var email = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(email))
                {
                    _unitOfWork.SetEmail(email);
                }
            }
        }

        /// <summary>
        /// Anonymous increment of a daily aggregate counter. Accepts only an
        /// allowlisted event name and, for visits, a referrer host that is
        /// immediately mapped to a bucket. Nothing in the request is stored raw.
        /// </summary>
        [HttpPost("hit")]
        [AllowAnonymous]
        [RequestSizeLimit(SiteFunnelBuckets.MaxBodyBytes)]
        [RequestFormLimits(ValueLengthLimit = SiteFunnelBuckets.MaxBodyBytes, MultipartBodyLengthLimit = SiteFunnelBuckets.MaxBodyBytes)]
        public async Task<IActionResult> Hit([FromBody] SiteFunnelHitRequest? body, CancellationToken cancellationToken)
        {
            if (Request.ContentLength is > SiteFunnelBuckets.MaxBodyBytes)
            {
                return StatusCode(StatusCodes.Status413PayloadTooLarge);
            }

            if (!SiteFunnelBuckets.TryResolveHit(body?.Event, body?.Referrer, out var bucket, out _))
            {
                return BadRequest();
            }

            await _funnel.IncrementAsync(bucket, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Daily funnel totals. Requires a Vectarr admin JWT or the SiteFunnel:ReadKey
        /// sent as an X-Metrics-Key header. The key must be set in server config;
        /// an empty key never grants access.
        /// </summary>
        [HttpGet("daily")]
        [AllowAnonymous]
        public async Task<ActionResult<SiteFunnelDailyResponse>> Daily(
            [FromQuery] DateOnly? from,
            [FromQuery] DateOnly? to,
            CancellationToken cancellationToken)
        {
            if (!CanReadMetrics())
            {
                return Unauthorized();
            }

            return Ok(await _funnel.GetDailyAsync(from, to, cancellationToken));
        }

        private bool CanReadMetrics()
        {
            var configured = _configuration[ReadKeyConfig];
            if (!string.IsNullOrEmpty(configured)
                && Request.Headers.TryGetValue(ReadKeyHeader, out var provided)
                && KeyMatches(configured, provided.ToString()))
            {
                return true;
            }

            return _admin.IsCurrentUserVectarrAdmin();
        }

        private static bool KeyMatches(string configured, string provided)
        {
            var left = Encoding.UTF8.GetBytes(configured);
            var right = Encoding.UTF8.GetBytes(provided);
            if (left.Length != right.Length)
            {
                return false;
            }

            return CryptographicOperations.FixedTimeEquals(left, right);
        }
    }
}
