using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.Model
{
    [Table("WorkshopSpecialization.WorkshopSpecializations")]
    public class WorkshopSpecialization : BaseModel
    {
        public int WorkshopId { get; set; }
        public virtual Workshop Workshop { get; set; }
        public WorkshopSpecializationEnum? Specialization { get; set; }
        public string? OtherSpecialization { get; set; }
    }
}
