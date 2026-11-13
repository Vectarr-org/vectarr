using Microsoft.Extensions.Configuration;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.Exceptions;

namespace Vectarr.Palooza.ApiBusiness.Services.BillingServices
{
    /// <summary>
    /// Reads the "Stripe" configuration section and creates API clients, so the quote payment and
    /// saved-card services resolve keys and currency the same way.
    /// </summary>
    public class StripeGateway
    {
        public const string NotConfiguredMessage = "Online payment is not configured yet.";

        private readonly IConfiguration _configuration;

        public StripeGateway(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public StripeConfig GetConfig()
        {
            var config = _configuration.GetSection(StripeConfig.SectionName).Get<StripeConfig>() ?? new StripeConfig();
            config.Currency = string.IsNullOrWhiteSpace(config.Currency) ? "usd" : config.Currency.Trim().ToLowerInvariant();
            return config;
        }

        public Stripe.StripeClient CreateClient()
        {
            var config = GetConfig();
            if (string.IsNullOrWhiteSpace(config.SecretKey))
            {
                throw new ErrorValidationException(NotConfiguredMessage);
            }

            return new Stripe.StripeClient(config.SecretKey.Trim());
        }

        /// <summary>The browser key paired with SecretKey; the payment forms cannot load without it.</summary>
        public string GetPublishableKey()
        {
            var key = GetConfig().PublishableKey?.Trim();
            if (string.IsNullOrEmpty(key))
            {
                throw new ErrorValidationException(NotConfiguredMessage);
            }

            return key;
        }

        /// <summary>Two-decimal currencies (usd): cents.</summary>
        public static long ToMinorUnits(decimal amount)
        {
            return (long)Math.Round(amount * 100m, 0, MidpointRounding.AwayFromZero);
        }

        public static decimal FromMinorUnits(long amount)
        {
            return amount / 100m;
        }
    }
}
