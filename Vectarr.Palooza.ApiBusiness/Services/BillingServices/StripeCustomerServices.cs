using System.Globalization;
using Microsoft.Extensions.Logging;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.Exceptions;

namespace Vectarr.Palooza.ApiBusiness.Services.BillingServices
{
    /// <summary>
    /// The signed in user's Stripe customer and saved cards. Nothing is stored on our side: the customer is
    /// found through its "vectarr_identity_id" metadata and cards only ever live in Stripe.
    /// </summary>
    public class StripeCustomerServices
    {
        private const string IdentityMetadataKey = "vectarr_identity_id";
        private const string CardNotFoundMessage = "This card was not found on your account.";

        private readonly IUnitOfWork _unitOfWork;
        private readonly StripeGateway _stripe;
        private readonly ILogger<StripeCustomerServices> _logger;

        public StripeCustomerServices(IUnitOfWork unitOfWork, StripeGateway stripe, ILogger<StripeCustomerServices> logger)
        {
            _unitOfWork = unitOfWork;
            _stripe = stripe;
            _logger = logger;
        }

        public bool IsSignedIn => _unitOfWork.CurrentUser != null && _unitOfWork.Identity != null;

        /// <summary>
        /// Stripe customer id of the signed in user. Null for guests, or when none exists yet and createIfMissing is false.
        /// </summary>
        public async Task<string?> GetCurrentCustomerId(bool createIfMissing)
        {
            if (!IsSignedIn)
            {
                return null;
            }

            var identity = _unitOfWork.Identity;
            var identityIdText = identity.Id.ToString(CultureInfo.InvariantCulture);
            var customers = new Stripe.CustomerService(_stripe.CreateClient());

            try
            {
                var found = await customers.SearchAsync(new Stripe.CustomerSearchOptions
                {
                    Query = $"metadata['{IdentityMetadataKey}']:'{identityIdText}'",
                    Limit = 1
                });

                var existing = found.Data.FirstOrDefault();
                if (existing != null || !createIfMissing)
                {
                    return existing?.Id;
                }

                var email = string.IsNullOrWhiteSpace(identity.EmailAddress) ? _unitOfWork.CurrentUser.Email : identity.EmailAddress.Trim();
                var name = string.Join(" ", new[] { identity.FirstName, identity.LastName }.Where(s => !string.IsNullOrWhiteSpace(s))).Trim();

                var created = await customers.CreateAsync(new Stripe.CustomerCreateOptions
                {
                    Email = email,
                    Name = string.IsNullOrEmpty(name) ? null : name,
                    Metadata = new Dictionary<string, string> { { IdentityMetadataKey, identityIdText } }
                }, new Stripe.RequestOptions
                {
                    // Search is eventually consistent; the key stops a second call made right after this one
                    // from creating a duplicate customer.
                    IdempotencyKey = "vectarr-customer-" + identityIdText
                });

                _logger.LogInformation("Created Stripe customer {CustomerId} for identity {IdentityId}.", created.Id, identity.Id);
                return created.Id;
            }
            catch (Stripe.StripeException ex)
            {
                _logger.LogError(ex, "Could not resolve the Stripe customer for identity {IdentityId}.", identity.Id);
                throw new ErrorValidationException("Could not reach the payment provider. Please try again in a moment.");
            }
        }

        /// <summary>Saved cards of the signed in user, newest first. Empty for guests.</summary>
        public async Task<List<SavedCardModel>> ListCards()
        {
            var customerId = await GetCurrentCustomerId(createIfMissing: false);
            if (customerId == null)
            {
                return new List<SavedCardModel>();
            }

            try
            {
                var methods = await new Stripe.CustomerPaymentMethodService(_stripe.CreateClient()).ListAsync(customerId,
                    new Stripe.CustomerPaymentMethodListOptions { Type = "card", Limit = 20 });

                return methods.Data
                    .Where(pm => pm.Card != null)
                    .Select(pm => new SavedCardModel
                    {
                        Id = pm.Id,
                        Brand = pm.Card.DisplayBrand ?? pm.Card.Brand ?? "card",
                        Last4 = pm.Card.Last4,
                        ExpMonth = (int)pm.Card.ExpMonth,
                        ExpYear = (int)pm.Card.ExpYear,
                        Wallet = pm.Card.Wallet?.Type
                    })
                    .ToList();
            }
            catch (Stripe.StripeException ex)
            {
                _logger.LogError(ex, "Could not list saved cards for Stripe customer {CustomerId}.", customerId);
                throw new ErrorValidationException("Could not load your saved cards. Please try again in a moment.");
            }
        }

        /// <summary>SetupIntent used by the Money Center to save a new card for later payments.</summary>
        public async Task<StripeSetupIntentModel> CreateSetupIntent()
        {
            var publishableKey = _stripe.GetPublishableKey();
            var customerId = await GetCurrentCustomerId(createIfMissing: true)
                ?? throw new ErrorValidationException("Sign in to save a card.");

            try
            {
                var intent = await new Stripe.SetupIntentService(_stripe.CreateClient()).CreateAsync(new Stripe.SetupIntentCreateOptions
                {
                    Customer = customerId,
                    PaymentMethodTypes = new List<string> { "card" },
                    Usage = "off_session"
                });

                return new StripeSetupIntentModel
                {
                    ClientSecret = intent.ClientSecret,
                    PublishableKey = publishableKey
                };
            }
            catch (Stripe.StripeException ex)
            {
                _logger.LogError(ex, "Could not create a SetupIntent for Stripe customer {CustomerId}.", customerId);
                throw new ErrorValidationException("Could not start adding a card. Please try again in a moment.");
            }
        }

        /// <summary>Detaches a saved card, only when it belongs to the signed in user's customer.</summary>
        public async Task RemoveCard(string? paymentMethodId)
        {
            if (string.IsNullOrWhiteSpace(paymentMethodId))
            {
                throw new ErrorValidationException(CardNotFoundMessage);
            }

            var customerId = await GetCurrentCustomerId(createIfMissing: false)
                ?? throw new ErrorValidationException(CardNotFoundMessage);

            var paymentMethods = new Stripe.PaymentMethodService(_stripe.CreateClient());
            try
            {
                var method = await paymentMethods.GetAsync(paymentMethodId.Trim());
                if (!string.Equals(method.CustomerId, customerId, StringComparison.Ordinal))
                {
                    throw new ErrorValidationException(CardNotFoundMessage);
                }

                await paymentMethods.DetachAsync(method.Id);
            }
            catch (Stripe.StripeException ex)
            {
                _logger.LogWarning(ex, "Could not remove payment method {PaymentMethodId} for Stripe customer {CustomerId}.", paymentMethodId, customerId);
                throw new ErrorValidationException(CardNotFoundMessage);
            }
        }
    }
}
