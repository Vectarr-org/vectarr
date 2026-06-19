using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vectarr.Palooza.Model
{
    [Table("Job.WorkOrderProgressUpdates")]
    public class WorkOrderProgressUpdate : BaseModel
    {
        public int ProgressStageId { get; set; }
        public virtual WorkOrderProgressStage ProgressStage { get; set; }
        public int PercentComplete { get; set; }
        public string? Notes { get; set; }
        public virtual WorkshopCustomProgressMessage? WorkshopCustomProgressMessage { get; set; }
        public int? WorkshopCustomProgressMessageId { get; set; }
        public virtual ICollection<Image> Images { get; set; }
    }
}
