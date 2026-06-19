using Vectarr.Palooza.CoreClasses;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.CoreClasses.Wrappers
{
    public class SubscriptionStatusWrapper : BaseEnumClass<SubscriptionStatusEnum>
    {
        public override string GetDisplayText(SubscriptionStatusEnum enumValue)
        {
            switch (enumValue)
            {
                case SubscriptionStatusEnum.Active:
                    return "Active";
                case SubscriptionStatusEnum.Canceled:
                    return "Canceled";
                default:
                    return "Active";
            }
        }
    }
}
