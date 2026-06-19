using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.CoreClasses.Wrappers;
using Vectarr.Palooza.Classes.BaseApi;

namespace Vectarr.Palooza.Classes.Api
{
    public class WorkshopSpecializationModel : BaseApiModel
    {
        public WorkshopSpecializationEnum? Specialization { get; set; }
        public string? OtherSpecialization { get; set; }
        public string? DisplayName => Specialization.HasValue ? new WorkshopSpecializationEnumWrapper().GetDisplayText(Specialization.Value) : null;
    }
}
