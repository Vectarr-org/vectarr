using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Vectarr.Palooza.Model
{
    [Table("Image.Images")]
    public class Image : BaseModel
    {
        [Required]
        public string? OriginalPath { get; set; }
        [StringLength(100)]
        public string? AltText { get; set; }
        public string? StorageUrl { get; set; }
        public string? RelativeThumbnailOriginalPath { get; set; }
        public int? WorkshopId { get; set; }
        public virtual Workshop? Workshop { get; set; }
        public int? JobId { get; set; }
        public virtual Job? Job { get; set; }
        public int? ProgressStageId { get; set; }
        public virtual WorkOrderProgressStage? ProgressStage { get; set; }
        public int? ProgressUpdateId { get; set; }
        public virtual WorkOrderProgressUpdate? ProgressUpdate { get; set; }
        public int? WorkOrderId { get; set; }
        public virtual WorkOrder? WorkOrder { get; set; }
        public virtual WorkOrderQualityCheckRequest? WorkOrderQualityCheckRequest { get; set; }
        public int? WorkOrderQualityCheckRequestId { get; set; }
    }
}
