namespace Vectarr.Palooza.Classes.Api
{
    /// <summary>
    /// A card saved in Stripe for the signed in user. Only display data: Vectarr never stores card details.
    /// </summary>
    public class SavedCardModel
    {
        /// <summary>Stripe PaymentMethod id (pm_...).</summary>
        public string Id { get; set; } = null!;
        public string Brand { get; set; } = null!;
        public string Last4 { get; set; } = null!;
        public int ExpMonth { get; set; }
        public int ExpYear { get; set; }

        /// <summary>Apple Pay / Google Pay etc. when the card was saved from a wallet.</summary>
        public string? Wallet { get; set; }
    }

    /// <summary>
    /// Response of POST api/Billing/CreateSetupIntent: what the Payment Element needs to save a new card.
    /// </summary>
    public class StripeSetupIntentModel
    {
        public string ClientSecret { get; set; } = null!;
        public string PublishableKey { get; set; } = null!;
    }
}
