namespace Vectarr.Palooza.Classes.Api
{
    /// <summary>
    /// Body of POST api/QuoteRequest/AdminUpdate. Null fields are left unchanged
    /// (send adminNotes: "" to clear the notes).
    /// </summary>
    public class QuoteRequestAdminUpdateModel
    {
        public int Id { get; set; }
        public int? Status { get; set; }
        public string? AdminNotes { get; set; }
    }

    /// <summary>
    /// Response of GET api/QuoteRequest/AdminStats.
    /// </summary>
    public class QuoteRequestStatsModel
    {
        public int Total { get; set; }
        public int AwaitingPayment { get; set; }
        public int Paid { get; set; }
        public int InReview { get; set; }
        public int SentToWorkshops { get; set; }
        public int Completed { get; set; }
        public int Cancelled { get; set; }
        public int Refunded { get; set; }

        /// <summary>Sum of AmountPaid over all requests that are not Refunded.</summary>
        public decimal PaidAmount { get; set; }
    }

    /// <summary>
    /// Response of POST api/QuoteRequest/CreatePaymentIntent: what the checkout page's card form and wallet buttons pay with.
    /// </summary>
    public class QuoteRequestPaymentIntentModel
    {
        public string ClientSecret { get; set; } = null!;
        public string PaymentIntentId { get; set; } = null!;
        public string PublishableKey { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "usd";
    }

    /// <summary>
    /// Response of GET api/QuoteRequest/MinePaymentSummary (the caller's own requests, same owner rule as Mine).
    /// </summary>
    public class QuoteRequestPaymentSummaryModel
    {
        /// <summary>Sum of AmountPaid over requests that are not Refunded.</summary>
        public decimal TotalPaid { get; set; }
        public int PaidCount { get; set; }

        /// <summary>Sum of AmountDue over requests still AwaitingPayment.</summary>
        public decimal AwaitingPaymentAmount { get; set; }
        public int AwaitingPaymentCount { get; set; }

        public DateTime? LastPaidAt { get; set; }
    }
}
