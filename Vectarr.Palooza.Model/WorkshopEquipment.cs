using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.Model
{
    [Table("WorkshopEquipment.WorkshopEquipments")]
    public class WorkshopEquipment : BaseModel
    {
        public int WorkshopId { get; set; }
        public virtual Workshop Workshop { get; set; }
        public WorkshopEquipmentEnum? Equipment { get; set; }
        public string? OtherEquipment { get; set; }
    }

}
