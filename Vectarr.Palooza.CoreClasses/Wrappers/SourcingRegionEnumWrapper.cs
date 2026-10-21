using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.CoreClasses.Wrappers
{
    public class SourcingRegionEnumWrapper : BaseEnumClass<SourcingRegionEnum>
    {
        public override string GetDisplayText(SourcingRegionEnum enumValue)
        {
            switch (enumValue)
            {
                case SourcingRegionEnum.UsShopsOnly:
                    return "US shops only";
                case SourcingRegionEnum.AnyRegion:
                    return "Any region";
                default:
                    return "";
            }
        }
    }
}
