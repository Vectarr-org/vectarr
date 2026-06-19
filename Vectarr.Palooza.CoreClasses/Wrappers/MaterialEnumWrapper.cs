using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.CoreClasses.Wrappers
{
    public class MaterialEnumWrapper : BaseEnumClass<MaterialEnum>
    {
        public override string GetDisplayText(MaterialEnum enumValue)
        {
            switch (enumValue)
            {
                case MaterialEnum.Aluminum:
                    return "Aluminum";
                case MaterialEnum.Steel:
                    return "Steel";
                case MaterialEnum.Wood:
                    return "Wood";
                case MaterialEnum.PlasticABS:
                    return "Plastic ABS";
                case MaterialEnum.PlasticPLA:
                    return "Plastic PLA";
                case MaterialEnum.PlasticNylon:
                    return "Plastic Nylon";
                case MaterialEnum.Brass:
                    return "Brass";
                case MaterialEnum.Copper:
                    return "Copper";
                case MaterialEnum.Titanium:
                    return "Titanium";
                case MaterialEnum.StanelessSteel:
                    return "Staneless Steel";
                default:
                    return "";
            }
        }
    }
}
