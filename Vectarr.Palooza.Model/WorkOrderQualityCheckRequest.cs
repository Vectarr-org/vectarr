using System.ComponentModel.DataAnnotations.Schema;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.Model
{
    [Table("WorkOrder.QualityCheckRequests")]
    public class WorkOrderQualityCheckRequest : BaseModel
    {
        public int WorkOrderId { get; set; }
        public virtual WorkOrder WorkOrder { get; set; }

        public QualityCheckTypeEnum CheckType { get; set; }
        public string? Instructions { get; set; } // Creator can add specific notes
        // Workshop response
        public bool? Passed { get; set; }
        public string? ResultNotes { get; set; }
        public DateTime? CompletedAt { get; set; }
        // Required attachment when creator requests this check
        public virtual ICollection<Image> Attachments { get; set; }
        public virtual ICollection<WorkOrderQualityCheckRejection> Rejections { get; set; }

    }
}
