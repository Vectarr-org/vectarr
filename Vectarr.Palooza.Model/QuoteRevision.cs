using System.ComponentModel.DataAnnotations.Schema;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.Model
{
    [Table("Job.QuoteRevisions")]
    public class QuoteRevision : BaseModel
    {
        public int WorkOrderId { get; set; }
        public virtual WorkOrder WorkOrder { get; set; }
        public decimal NewPrice { get; set; }
        public decimal OldPrice { get; set; }
        public DateTime? NewDeadline { get; set; }
        public DateTime? OldDeadline { get; set; }
        public string? Reason { get; set; }
        public string? ChangeSummary { get; set; }
        public RevisionStatusEnum Status { get; set; }
        public DateTime? RespondedAt { get; set; }
        public int? RespondedByCreatorId { get; set; }
        public virtual Creator? RespondedByCreator { get; set; }
    }
}
