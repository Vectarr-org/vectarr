using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.CoreClasses.Wrappers
{
    public class CostVsSpeedEnumWrapper : BaseEnumClass<CostVsSpeedEnum>
    {
        public override string GetDisplayText(CostVsSpeedEnum enumValue)
        {
            switch (enumValue)
            {
                case CostVsSpeedEnum.Cheapest:
                    return "Cheapest";
                case CostVsSpeedEnum.LowSoon:
                    return "Low & Soon";
                case CostVsSpeedEnum.Fair:
                    return "Fair";
                case CostVsSpeedEnum.Fast:
                    return "Fast";
                case CostVsSpeedEnum.Fastest:
                    return "Fastest";
                default:
                    return "Cost";
            }
        }

        public string GetDisplayDescription(CostVsSpeedEnum enumValue)
        {
            switch (enumValue)
            {
                case CostVsSpeedEnum.Cheapest:
                    return "Cheapest, take your time";
                case CostVsSpeedEnum.LowSoon:
                    return "Low prices, but I need it soon";
                case CostVsSpeedEnum.Fair:
                    return "Fair price, Fair timing";
                case CostVsSpeedEnum.Fast:
                    return "I need it fast, but I'll pay a little more";
                case CostVsSpeedEnum.Fastest:
                    return "I'll pay for speed";
                default:
                    return "Cost";
            }
        }
    }
}
