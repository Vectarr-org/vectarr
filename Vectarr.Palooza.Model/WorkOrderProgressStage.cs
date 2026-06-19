using System.ComponentModel.DataAnnotations.Schema;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.Model
{
    [Table("Job.WorkOrderProgressStages")]
    public class WorkOrderProgressStage : BaseModel
    {
        public int WorkOrderId { get; set; }
        public virtual WorkOrder WorkOrder { get; set; }
        public ProgressStageEnum Stage { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? Notes { get; set; }
        public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
        public bool? PassedQualityCheck { get; set; }
        public string? QualityCheckNotes { get; set; }
        public virtual ICollection<Image> StageImages { get; set; }
        public DateTime? WorkshopSubmittedAt { get; set; }
        public ConditionEnum? Condition { get; set; }
        public virtual ICollection<WorkOrderProgressUpdate> ProgressUpdates { get; set; }
    }
}
