using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vectarr.Palooza.Model
{
    [Table("Workshop.WorkshopCustomProgressMessages")]
    public class WorkshopCustomProgressMessage : BaseModel
    {
        public string Text { get; set; }
        public int WorkshopId { get; set; }
        public virtual Workshop Workshop { get; set; }
        public virtual ICollection<WorkOrderProgressUpdate> WorkOrderProgressUpdates { get; set; }

    }
}
