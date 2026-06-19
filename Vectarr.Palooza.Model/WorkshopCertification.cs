using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.Model
{
    [Table("WorkshopCertification.WorkshopCertifications")]
    public class WorkshopCertification : BaseModel
    {
        public int WorkshopId { get; set; }
        public virtual Workshop Workshop { get; set; }
        public WorkshopCertificationEnum? Certification { get; set; }
        public string? OtherCertification { get; set; }
    }
}
