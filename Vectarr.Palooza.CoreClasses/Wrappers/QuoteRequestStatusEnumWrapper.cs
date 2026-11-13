using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.CoreClasses.Wrappers
{
    public class QuoteRequestStatusEnumWrapper : BaseEnumClass<QuoteRequestStatusEnum>
    {
        public override string GetDisplayText(QuoteRequestStatusEnum enumValue)
        {
            switch (enumValue)
            {
                case QuoteRequestStatusEnum.AwaitingPayment:
                    return "Awaiting Payment";
                case QuoteRequestStatusEnum.Paid:
                    return "Paid";
                case QuoteRequestStatusEnum.InReview:
                    return "In Review";
                case QuoteRequestStatusEnum.SentToWorkshops:
                    return "Sent To Workshops";
                case QuoteRequestStatusEnum.Completed:
                    return "Completed";
                case QuoteRequestStatusEnum.Cancelled:
                    return "Cancelled";
                case QuoteRequestStatusEnum.Refunded:
                    return "Refunded";
                default:
                    return "";
            }
        }
    }
}
