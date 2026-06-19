using Vectarr.Palooza.CoreClasses;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.CoreClasses.Wrappers
{
    public class VectarrSubscriptionWrapper : BaseEnumClass<VectarrSubscriptiontEnum>
    {
        public override string GetDisplayText(VectarrSubscriptiontEnum enumValue)
        {
            switch (enumValue)
            {
                default:
                    return "Discover Plan";
            }
        }

        public decimal SubscriptionPrice(VectarrSubscriptiontEnum subscription)
        {
            switch (subscription)
            {

                default:
                    return 0;
            }
        }

        public string StripeProduct(VectarrSubscriptiontEnum subscription)
        {
            switch (subscription)
            {

                default:
                    return "";
            }
        }
    }
}
