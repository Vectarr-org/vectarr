using System.Globalization;
using Microsoft.Extensions.Logging;
using Vectarr.Palooza.ApiBusiness.Services.BillingServices;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.Exceptions;
using Vectarr.Palooza.Model;
using StripeCheckout = Stripe.Checkout;

namespace Vectarr.Palooza.ApiBusiness.Services.QuoteRequestServices
{
    /// <summary>
    /// Stripe payments for instant quote requests, taken on our own checkout page with the embedded Payment Element:
    /// PaymentIntent creation, confirmation after payment and the signed webhook. Marking a request as paid is
    /// delegated to QuoteRequestServices.MarkPaid, which is idempotent, so the success page and the webhook can both
    /// confirm the same payment. Checkout Sessions created before the switch to the Payment Element still confirm.
    /// </summary>
    public class QuoteRequestPaymentServices
    {
        private const string PaymentIntentSucceeded = "payment_intent.succeeded";
        private const string CheckoutSessionCompleted = "checkout.session.completed";
        private const string CheckoutSessionAsyncPaymentSucceeded = "checkout.session.async_payment_succeeded";
        private const string QuoteRequestIdMetadataKey = "quoteRequestId";
        private const string ReferenceNumberMetadataKey = "referenceNumber";
        private const char EnDash = (char)0x2013;
        private const char MiddleDot = (char)0x00B7;

        /// <summary>PaymentIntent states in which the buyer can still (re)try paying with the same intent.</summary>
        private static readonly string[] OpenIntentStatuses = { "requires_payment_method", "requires_confirmation", "requires_action" };

        private readonly IUnitOfWork _unitOfWork;
        private readonly StripeGateway _stripe;
        private readonly StripeCustomerServices _customers;
        private readonly QuoteRequestServices _quoteRequestServices;
        private readonly ILogger<QuoteRequestPaymentServices> _logger;

        public QuoteRequestPaymentServices(IUnitOfWork unitOfWork, StripeGateway stripe, StripeCustomerServices customers,
            QuoteRequestServices quoteRequestServices, ILogger<QuoteRequestPaymentServices> logger)
        {
            _unitOfWork = unitOfWork;
            _stripe = stripe;
            _customers = customers;
            _quoteRequestServices = quoteRequestServices;
            _logger = logger;
        }

        /// <summary>
        /// PaymentIntent for the request's AmountDue, reused while it can still be paid so retries never create a
        /// second charge. Signed in buyers pay as their Stripe customer, so they can pay with or save a card.
        /// Null when the request is not found / not allowed.
        /// </summary>
        public async Task<QuoteRequestPaymentIntentModel?> CreatePaymentIntent(int id, string? token)
        {
            var entity = _quoteRequestServices.GetAuthorizedEntity(id, token);
            if (entity == null)
            {
                return null;
            }

            if (entity.Status != QuoteRequestStatusEnum.AwaitingPayment)
            {
                throw new ErrorValidationException("This quote request is not awaiting payment.");
            }

            if (entity.AmountDue <= 0)
            {
                throw new ErrorValidationException("This quote request has no amount due.");
            }

            var publishableKey = _stripe.GetPublishableKey();
            var currency = string.IsNullOrWhiteSpace(entity.Currency) ? _stripe.GetConfig().Currency : entity.Currency.Trim().ToLowerInvariant();
            var amount = StripeGateway.ToMinorUnits(entity.AmountDue);
            var customerId = await _customers.GetCurrentCustomerId(createIfMissing: true);
            var intents = new Stripe.PaymentIntentService(_stripe.CreateClient());

            var intent = await FindOpenIntent(intents, entity, amount, currency, customerId);
            if (intent == null)
            {
                var idText = entity.Id.ToString(CultureInfo.InvariantCulture);
                try
                {
                    intent = await intents.CreateAsync(new Stripe.PaymentIntentCreateOptions
                    {
                        Amount = amount,
                        Currency = currency,
                        Customer = customerId,
                        AutomaticPaymentMethods = new Stripe.PaymentIntentAutomaticPaymentMethodsOptions { Enabled = true },
                        Description = $"Vectarr instant quote {entity.ReferenceNumber} {EnDash} initial payment ({BuildDescription(entity.PartName, entity.Quantity, entity.MaterialName)})",
                        Metadata = new Dictionary<string, string>
                        {
                            { QuoteRequestIdMetadataKey, idText },
                            { ReferenceNumberMetadataKey, entity.ReferenceNumber }
                        }
                    });
                }
                catch (Stripe.StripeException ex)
                {
                    _logger.LogError(ex, "Stripe PaymentIntent creation failed for quote request {Id}.", entity.Id);
                    throw new ErrorValidationException("Could not start the online payment: " + (ex.StripeError?.Message ?? ex.Message));
                }

                entity.StripePaymentIntentId = intent.Id;
                _unitOfWork.Repositories.QuoteRequestRepository.Update(entity);
            }

            return new QuoteRequestPaymentIntentModel
            {
                ClientSecret = intent.ClientSecret,
                PaymentIntentId = intent.Id,
                PublishableKey = publishableKey,
                Amount = entity.AmountDue,
                Currency = currency
            };
        }

        /// <summary>
        /// Called by the success page: verifies the payment with Stripe and marks the request paid (idempotent).
        /// Pass the PaymentIntent id (Payment Element) or, for older orders, the Checkout Session id.
        /// Null when the request is not found / not allowed.
        /// </summary>
        public async Task<QuoteRequestModel?> ConfirmPayment(int id, string? token, string? paymentIntentId, string? sessionId)
        {
            var entity = _quoteRequestServices.GetAuthorizedEntity(id, token);
            if (entity == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(paymentIntentId) && string.IsNullOrWhiteSpace(sessionId))
            {
                throw new ErrorValidationException("paymentIntentId is required.");
            }

            if (!entity.PaidAt.HasValue)
            {
                var client = _stripe.CreateClient();
                if (!string.IsNullOrWhiteSpace(paymentIntentId))
                {
                    await ConfirmFromIntent(client, entity, paymentIntentId.Trim());
                }
                else
                {
                    await ConfirmFromSession(client, entity, sessionId!.Trim());
                }
            }

            return _quoteRequestServices.GetModelForCaller(entity, token);
        }

        /// <summary>
        /// Handles a Stripe webhook call. Returns false when the signature is invalid (answer 400);
        /// true for handled and ignored events (answer 200).
        /// </summary>
        public bool HandleWebhook(string json, string? signatureHeader)
        {
            var stripeConfig = _stripe.GetConfig();
            if (string.IsNullOrWhiteSpace(stripeConfig.WebhookSecret))
            {
                throw new ErrorValidationException(StripeGateway.NotConfiguredMessage);
            }

            Stripe.Event stripeEvent;
            try
            {
                stripeEvent = Stripe.EventUtility.ConstructEvent(json, signatureHeader ?? string.Empty, stripeConfig.WebhookSecret, throwOnApiVersionMismatch: false);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Rejected Stripe webhook call (invalid signature or payload).");
                return false;
            }

            if (stripeEvent.Type == PaymentIntentSucceeded && stripeEvent.Data?.Object is Stripe.PaymentIntent intent)
            {
                HandlePaidIntent(stripeEvent.Id, intent);
            }
            else if ((stripeEvent.Type == CheckoutSessionCompleted || stripeEvent.Type == CheckoutSessionAsyncPaymentSucceeded)
                && stripeEvent.Data?.Object is StripeCheckout.Session session)
            {
                HandlePaidSession(stripeEvent.Id, session);
            }

            return true;
        }

        #region PaymentIntents

        /// <summary>
        /// The request's current PaymentIntent when it can still be paid for the same amount, currency and customer.
        /// A mismatching open intent is cancelled; a succeeded or processing one stops a second charge.
        /// </summary>
        private async Task<Stripe.PaymentIntent?> FindOpenIntent(Stripe.PaymentIntentService intents, QuoteRequest entity, long amount, string currency, string? customerId)
        {
            if (string.IsNullOrWhiteSpace(entity.StripePaymentIntentId))
            {
                return null;
            }

            Stripe.PaymentIntent existing;
            try
            {
                existing = await intents.GetAsync(entity.StripePaymentIntentId);
            }
            catch (Stripe.StripeException ex)
            {
                _logger.LogWarning(ex, "Could not retrieve PaymentIntent {IntentId} of quote request {Id}; creating a new one.", entity.StripePaymentIntentId, entity.Id);
                return null;
            }

            if (existing.Status == "succeeded")
            {
                if (IntentBelongsTo(existing, entity.Id, entity.ReferenceNumber))
                {
                    MarkPaidFromIntent(entity.Id, existing);
                }
                throw new ErrorValidationException("This quote request is already paid.");
            }

            if (existing.Status == "processing")
            {
                throw new ErrorValidationException("A payment for this quote request is already processing. We will email you as soon as it clears.");
            }

            if (!OpenIntentStatuses.Contains(existing.Status))
            {
                return null;
            }

            var reusable = existing.Amount == amount
                && string.Equals(existing.Currency, currency, StringComparison.OrdinalIgnoreCase)
                && string.Equals(existing.CustomerId, customerId, StringComparison.Ordinal);
            if (reusable)
            {
                return existing;
            }

            try
            {
                await intents.CancelAsync(existing.Id);
            }
            catch (Stripe.StripeException ex)
            {
                _logger.LogWarning(ex, "Could not cancel replaced PaymentIntent {IntentId} of quote request {Id}.", existing.Id, entity.Id);
            }

            return null;
        }

        private async Task ConfirmFromIntent(Stripe.StripeClient client, QuoteRequest entity, string paymentIntentId)
        {
            Stripe.PaymentIntent intent;
            try
            {
                intent = await new Stripe.PaymentIntentService(client).GetAsync(paymentIntentId);
            }
            catch (Stripe.StripeException ex)
            {
                _logger.LogWarning(ex, "Could not retrieve PaymentIntent {IntentId} for quote request {Id}.", paymentIntentId, entity.Id);
                throw new ErrorValidationException("Could not verify the payment with Stripe.");
            }

            if (!IntentBelongsTo(intent, entity.Id, entity.ReferenceNumber))
            {
                throw new ErrorValidationException("This payment does not belong to this quote request.");
            }

            if (intent.Status == "succeeded")
            {
                MarkPaidFromIntent(entity.Id, intent);
            }
        }

        private void HandlePaidIntent(string eventId, Stripe.PaymentIntent intent)
        {
            var quoteRequestId = ResolveQuoteRequestId(intent.Metadata);
            if (!quoteRequestId.HasValue)
            {
                // Not one of ours (e.g. another integration on the same Stripe account).
                return;
            }

            var entity = _unitOfWork.Repositories.QuoteRequestRepository.GetEntityById(quoteRequestId.Value, skipApplicationCheck: true);
            if (entity == null || !IntentBelongsTo(intent, entity.Id, entity.ReferenceNumber))
            {
                _logger.LogWarning("Stripe event {EventId}: PaymentIntent {IntentId} does not match quote request {Id}.", eventId, intent.Id, quoteRequestId.Value);
                return;
            }

            MarkPaidFromIntent(entity.Id, intent);
        }

        private void MarkPaidFromIntent(int quoteRequestId, Stripe.PaymentIntent intent)
        {
            var received = intent.AmountReceived > 0 ? intent.AmountReceived : intent.Amount;
            _quoteRequestServices.MarkPaid(quoteRequestId, StripeGateway.FromMinorUnits(received), intent.Id, null);
        }

        private static bool IntentBelongsTo(Stripe.PaymentIntent intent, int quoteRequestId, string referenceNumber)
        {
            return MetadataMatches(intent.Metadata, quoteRequestId, referenceNumber, clientReferenceId: null);
        }

        #endregion

        #region Checkout Sessions (orders started before the Payment Element)

        private async Task ConfirmFromSession(Stripe.StripeClient client, QuoteRequest entity, string sessionId)
        {
            StripeCheckout.Session session;
            try
            {
                session = await new StripeCheckout.SessionService(client).GetAsync(sessionId);
            }
            catch (Stripe.StripeException ex)
            {
                _logger.LogWarning(ex, "Could not retrieve Stripe checkout session for quote request {Id}.", entity.Id);
                throw new ErrorValidationException("Could not verify the payment with Stripe.");
            }

            if (!SessionBelongsTo(session, entity.Id, entity.ReferenceNumber))
            {
                throw new ErrorValidationException("This payment session does not belong to this quote request.");
            }

            if (IsPaid(session))
            {
                MarkPaidFromSession(entity.Id, session);
            }
        }

        private void HandlePaidSession(string eventId, StripeCheckout.Session session)
        {
            if (!IsPaid(session))
            {
                // e.g. delayed payment methods: wait for checkout.session.async_payment_succeeded
                _logger.LogInformation("Stripe event {EventId} for session {SessionId} is not paid yet ({PaymentStatus}).", eventId, session.Id, session.PaymentStatus);
                return;
            }

            var quoteRequestId = ResolveQuoteRequestId(session.Metadata);
            if (!quoteRequestId.HasValue)
            {
                return;
            }

            var entity = _unitOfWork.Repositories.QuoteRequestRepository.GetEntityById(quoteRequestId.Value, skipApplicationCheck: true);
            if (entity == null || !SessionBelongsTo(session, entity.Id, entity.ReferenceNumber))
            {
                _logger.LogWarning("Stripe event {EventId}: session {SessionId} does not match quote request {Id}.", eventId, session.Id, quoteRequestId.Value);
                return;
            }

            MarkPaidFromSession(entity.Id, session);
        }

        private void MarkPaidFromSession(int quoteRequestId, StripeCheckout.Session session)
        {
            _quoteRequestServices.MarkPaid(quoteRequestId, StripeGateway.FromMinorUnits(session.AmountTotal ?? 0), session.PaymentIntentId, session.Id);
        }

        private static bool IsPaid(StripeCheckout.Session session)
        {
            return string.Equals(session.PaymentStatus, "paid", StringComparison.OrdinalIgnoreCase);
        }

        private static bool SessionBelongsTo(StripeCheckout.Session session, int quoteRequestId, string referenceNumber)
        {
            return MetadataMatches(session.Metadata, quoteRequestId, referenceNumber, session.ClientReferenceId);
        }

        #endregion

        #region Helpers

        private static int? ResolveQuoteRequestId(IDictionary<string, string>? metadata)
        {
            if (metadata != null
                && metadata.TryGetValue(QuoteRequestIdMetadataKey, out var metadataId)
                && int.TryParse(metadataId, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
            {
                return id;
            }

            return null;
        }

        /// <summary>
        /// The Stripe object must carry our metadata (quoteRequestId, or the session's client_reference_id)
        /// and the matching reference number.
        /// </summary>
        private static bool MetadataMatches(IDictionary<string, string>? metadata, int quoteRequestId, string referenceNumber, string? clientReferenceId)
        {
            var idText = quoteRequestId.ToString(CultureInfo.InvariantCulture);

            var idMatches = string.Equals(clientReferenceId, idText, StringComparison.Ordinal)
                || (metadata != null
                    && metadata.TryGetValue(QuoteRequestIdMetadataKey, out var metadataId)
                    && string.Equals(metadataId, idText, StringComparison.Ordinal));

            var referenceMatches = metadata != null
                && metadata.TryGetValue(ReferenceNumberMetadataKey, out var metadataReference)
                && string.Equals(metadataReference, referenceNumber, StringComparison.Ordinal);

            return idMatches && referenceMatches;
        }

        private static string BuildDescription(string? partName, int quantity, string? materialName)
        {
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(partName))
            {
                parts.Add(partName.Trim());
            }
            parts.Add("qty " + quantity.ToString(CultureInfo.InvariantCulture));
            if (!string.IsNullOrWhiteSpace(materialName))
            {
                parts.Add(materialName.Trim());
            }

            var description = string.Join(" " + MiddleDot + " ", parts);
            return description.Length <= 500 ? description : description.Substring(0, 500);
        }

        #endregion
    }
}
