namespace Vectarr.Palooza.Classes.Api
{
    /// <summary>
    /// Bound from the "InstantQuote" configuration section.
    /// </summary>
    public class InstantQuoteConfig
    {
        public const string SectionName = "InstantQuote";

        /// <summary>Percentage of the quote total charged up front (clamped to 0-100).</summary>
        public decimal InitialFeePercent { get; set; } = 100;

        /// <summary>Minimum initial payment.</summary>
        public decimal MinimumCharge { get; set; } = 1.00m;

        /// <summary>Maximum quote total accepted from the browser.</summary>
        public decimal MaxTotal { get; set; } = 500000m;

        /// <summary>Public site URL used for links.</summary>
        public string SiteUrl { get; set; } = "https://vectarr.com";

        /// <summary>Who gets notified when a quote request is paid.</summary>
        public List<string> AdminNotificationEmails { get; set; } = new List<string>();
    }

    /// <summary>
    /// Bound from the "Stripe" configuration section.
    /// </summary>
    public class StripeConfig
    {
        public const string SectionName = "Stripe";

        public string? SecretKey { get; set; }

        /// <summary>Browser key (pk_...) of the same Stripe account and mode as SecretKey; sent to the payment forms.</summary>
        public string? PublishableKey { get; set; }

        public string? WebhookSecret { get; set; }
        public string Currency { get; set; } = "usd";
    }
}
