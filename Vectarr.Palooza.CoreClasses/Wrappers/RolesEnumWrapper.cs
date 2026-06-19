using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.CoreClasses;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.CoreClasses.Wrappers
{
    public class RolesEnumWrapper : BaseEnumClass<RolesEnum>
    {
        public override string GetDisplayText(RolesEnum enumValue)
        {
            switch (enumValue)
            {
                case RolesEnum.Admin:
                    return "Admin";
                case RolesEnum.Creator:
                    return "Creator";
                case RolesEnum.Workshop:
                    return "Workshop";
                default:
                    return "";
            }
        }
    }
}
