using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.CoreClasses.Wrappers
{
    public class SourceFinishEnumWrapper : BaseEnumClass<SourceFinishEnum>
    {
        public override string GetDisplayText(SourceFinishEnum enumValue)
        {
            switch (enumValue)
            {
                case SourceFinishEnum.Anodized:
                    return "Anodized";
                    case SourceFinishEnum.Painted:
                    return "Painted";
                    case SourceFinishEnum.PowederCoated:
                    return "Powder Coated";
                    case SourceFinishEnum.Brushed:
                    return "Brushed";
                    case SourceFinishEnum.Polished:
                    return "Polished";
                    case SourceFinishEnum.AsMachined:
                    return "As Machined";
                default:
                    return "";
            }
        }
    }
}
