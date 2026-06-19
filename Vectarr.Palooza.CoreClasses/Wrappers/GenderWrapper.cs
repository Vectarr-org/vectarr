using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.CoreClasses.Wrappers
{
    public class GenderWrapper : BaseEnumClass<GenderEnum>
    {
        public override string GetDisplayText(GenderEnum enumValue) =>
            enumValue switch
            {
                GenderEnum.Male => "Male",
                GenderEnum.Female => "Female",
                GenderEnum.PreferNotToSay => "Prefer not to say",
                _ => ""
            };
    }
}
