using System.ComponentModel.DataAnnotations.Schema;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.Model
{
    [Table("ApplicationSubscription.ApplicationSubscriptions")]
    public class ApplicationSubscription : BaseModel
    {
        public virtual Application Application { get; set; }
        public VectarrSubscriptiontEnum Subscription { get; set; }
        public DateTime BillingCycleStartDate { get; set; }
        public DateTime? PaymentsFailureStartDate { get; set; }
        public SubscriptionStatusEnum SubscriptionStatus { get; set; }

        public CancelReasonEnum? CancelReason { get; set; }
        public decimal SubscriptionPrice { get; set; }

    }
}
