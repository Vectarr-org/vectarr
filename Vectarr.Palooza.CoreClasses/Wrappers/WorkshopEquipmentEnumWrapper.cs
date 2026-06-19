using Vectarr.Palooza.CoreClasses;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.CoreClasses.Wrappers
{
    public class WorkshopEquipmentEnumWrapper : BaseEnumClass<WorkshopEquipmentEnum>
    {
        public override string GetDisplayText(WorkshopEquipmentEnum enumValue)
        {
            switch (enumValue)
            {
                case WorkshopEquipmentEnum.ThreeAxisCNC:
                    return "3 Axis CNC";
                case WorkshopEquipmentEnum.FiveAxisCNC:
                    return "5 Axis CNC";
                case WorkshopEquipmentEnum.FDMPrinter:
                    return "FDM Printer";
                case WorkshopEquipmentEnum.SLAPrinter:
                    return "SLA Printer";
                case WorkshopEquipmentEnum.LaserCutter:
                    return "Laser Cutter";
                case WorkshopEquipmentEnum.PlasmaCutter:
                    return "Plasma Cutter";
                case WorkshopEquipmentEnum.MIGWelder:
                    return "MIG Welder";
                case WorkshopEquipmentEnum.TIGWelder:
                    return "TIG Welder";
                case WorkshopEquipmentEnum.Other:
                    return "Other";
                default:
                    return "";
            }
        }
    }
}
