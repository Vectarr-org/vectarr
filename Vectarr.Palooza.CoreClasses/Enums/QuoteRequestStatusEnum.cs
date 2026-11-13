namespace Vectarr.Palooza.CoreClasses.Enums
{
    /// <summary>
    /// Lifecycle of an instant quote request. Numeric values are part of the API contract.
    /// </summary>
    public enum QuoteRequestStatusEnum
    {
        AwaitingPayment = 0,
        Paid = 1,
        InReview = 2,
        SentToWorkshops = 3,
        Completed = 4,
        Cancelled = 5,
        Refunded = 6
    }
}
