using System.ComponentModel.DataAnnotations.Schema;

namespace Vectarr.Palooza.Model
{
    [Table("WorkOrder.WorkOrderQualityCheckRejections")]
    public class WorkOrderQualityCheckRejection : BaseModel
    {
        public string? RejectionReason { get; set; }
        public int QualityCheckRequestId { get; set; }
        public virtual WorkOrderQualityCheckRequest QualityCheckRequest { get; set; }
    }
}
