using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.Classes.BaseApi;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.Classes.Api
{
    public class WorkOrderProgressStageModel : BaseApiModel
    {
        public int WorkOrderId { get; set; }
        public ProgressStageEnum Stage { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? Notes { get; set; }
        public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
        public bool? PassedQualityCheck { get; set; }
        public string? QualityCheckNotes { get; set; }
        public DateTime? WorkshopSubmittedAt { get; set; }
        public List<ImageModel> StageImages { get; set; }
        public List<WorkOrderProgressUpdateModel>? ProgressUpdates { get; set; }
        public ConditionEnum? Condition { get; set; }
    }
}
