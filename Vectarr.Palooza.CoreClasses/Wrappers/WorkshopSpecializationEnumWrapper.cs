using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.CoreClasses;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.CoreClasses.Wrappers
{
    public class WorkshopSpecializationEnumWrapper : BaseEnumClass<WorkshopSpecializationEnum>
    {

        public override string GetDisplayText(WorkshopSpecializationEnum enumValue)
        {
            switch (enumValue)
            {
                case WorkshopSpecializationEnum.Aerospace:
                    return "Aerospace";
                case WorkshopSpecializationEnum.Automotive:
                    return "Automotive";
                case WorkshopSpecializationEnum.MedicalDevices:
                    return "Medical Devices";
                case WorkshopSpecializationEnum.Electronics:
                    return "Electronics";
                case WorkshopSpecializationEnum.Industrial:
                    return "Industrial";
                case WorkshopSpecializationEnum.Consumer:
                    return "Consumer";
                case WorkshopSpecializationEnum.Robotics:
                    return "Robotics";
                case WorkshopSpecializationEnum.Other:
                    return "Other";
                default:
                    return "";
            }
        }
    }
}
