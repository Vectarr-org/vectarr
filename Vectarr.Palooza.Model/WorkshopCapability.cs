using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.Model
{
    [Table("WorkshopCapability.WorkshopCapabilities")]
    public class WorkshopCapability : BaseModel
    {
        public int WorkshopId { get; set; }
        public virtual Workshop Workshop { get; set; }
        public ManufacturingTypeEnum? Capability { get; set; }
        public string? OtherCapability { get; set; } // filled when Capability = Other
    }
}
