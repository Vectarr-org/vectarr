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
    public class WorkshopEquipmentModel : BaseApiModel
    {
        public WorkshopEquipmentEnum? Equipment { get; set; }
        public string? OtherEquipment { get; set; }
        public string? DisplayName => Equipment.HasValue ? new WorkshopEquipmentEnumWrapper().GetDisplayText(Equipment.Value) : null;
    }
}
