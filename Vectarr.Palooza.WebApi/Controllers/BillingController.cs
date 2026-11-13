using Microsoft.AspNetCore.Mvc;
using Vectarr.Palooza.ApiBusiness.Services.BillingServices;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.DataAccess;

namespace Vectarr.Palooza.WebApi.Controllers
{
    /// <summary>
    /// Saved cards of the signed in user (Money Center). Cards live in Stripe only; the API returns
    /// brand, last 4 digits and expiry for display.
    /// </summary>
    [Route("api/[controller]")]
    public class BillingController : SystemController
    {
        private readonly StripeCustomerServices _customerServices;

        public BillingController(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, StripeCustomerServices customerServices)
            : base(unitOfWork, httpContextAccessor)
        {
            _customerServices = customerServices;
        }

        [HttpGet]
        [Route("PaymentMethods")]
        public async Task<ActionResult<List<SavedCardModel>>> PaymentMethods()
        {
            return await _customerServices.ListCards();
        }

        /// <summary>Returns the SetupIntent client secret (+ publishable key) used to save a new card.</summary>
        [HttpPost]
        [Route("CreateSetupIntent")]
        public async Task<ActionResult<StripeSetupIntentModel>> CreateSetupIntent()
        {
            return await _customerServices.CreateSetupIntent();
        }

        [HttpPost]
        [Route("RemovePaymentMethod")]
        public async Task<IActionResult> RemovePaymentMethod([FromQuery] string? id)
        {
            await _customerServices.RemoveCard(id);
            return Ok();
        }
    }
}
