using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.CoreClasses.Wrappers;
using Vectarr.Palooza.Classes.BaseApi;

namespace Vectarr.Palooza.Classes.Api
{
    public class ApplicationSubscriptionModel : BaseApiModel
    {
        public VectarrSubscriptiontEnum Subscription { get; set; }
        public DateTime BillingCycleStartDate { get; set; }
        public DateTime NextBillingDate => BillingCycleStartDate.AddMonths(1);
        public DateTime? PaymentsFailureStartDate { get; set; }
        public decimal SubscriptionPrice { get; set; }
        public SubscriptionStatusEnum SubscriptionStatus { get; set; }
        public string StatusName => new SubscriptionStatusWrapper().GetDisplayText(SubscriptionStatus);
        public string SusbcriptionName => new VectarrSubscriptionWrapper().GetDisplayText(Subscription);
        public string ISOUTCNextBillingDate => NextBillingDate.ToString("yyyy-MM-ddTHH:mm:ssZ");
        public CancelReasonEnum? CancelReason { get; set; }
        public string? UserEmail { get; set; }
    }
}
