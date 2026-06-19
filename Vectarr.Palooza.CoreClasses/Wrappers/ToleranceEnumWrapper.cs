using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.CoreClasses.Wrappers
{
    public class ToleranceEnumWrapper : BaseEnumClass<ToleranceEnum>
    {
        public override string GetDisplayText(ToleranceEnum enumValue)
        {
            switch (enumValue) {
                case ToleranceEnum.Standard:
                    return "Standard";
                case ToleranceEnum.Tight:
                    return "Tight";
                case ToleranceEnum.Precision:
                    return "Precision";
                default:
                    return "";
            }
        }
    }
}
