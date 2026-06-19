using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.CoreClasses.Wrappers
{
    public class ManufacturingTypeEnumWrapper : BaseEnumClass<ManufacturingTypeEnum>
    {
        public override string GetDisplayText(ManufacturingTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case ManufacturingTypeEnum.SheetMetalFabrication:
                    return "Sheet Metal Fabrication";
                case ManufacturingTypeEnum.CNCMachining:
                    return "CNC Machining";
                case ManufacturingTypeEnum.LaserCutting:
                    return "Laser Cutting";
                case ManufacturingTypeEnum.WaterjetCutting:
                    return "Waterjet Cutting";
                case ManufacturingTypeEnum.ThreeDPrinting:
                    return "3D Printing";
                case ManufacturingTypeEnum.InjectionMolding:
                    return "Injection Molding";
                    case ManufacturingTypeEnum.CastingForging:
                    return "Casting Forging";
                case ManufacturingTypeEnum.Welding:
                    return "Welding";
                case ManufacturingTypeEnum.Other:
                    return "Other";
                default:
                    return "";
            }
        }
    }
}
