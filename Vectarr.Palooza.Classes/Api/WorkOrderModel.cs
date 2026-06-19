using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.Classes.BaseApi;
using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.CoreClasses.Wrappers;

namespace Vectarr.Palooza.Classes.Api
{
    public class WorkOrderModel : BaseApiModel
    {
        public int JobId { get; set; }
        public JobModel? Job { get; set; }
        public int AcceptedBidId { get; set; }
        public BidModel AcceptedBid { get; set; }
        public int CreatorId { get; set; }
        public int WorkshopId { get; set; }
        public decimal CurrentPrice { get; set; }
        public DateTime? CurrentDeadline { get; set; }
        public int EstimatedDaysToComplete { get; set; }
        public DateTime? StartedDate { get; set; }
        public int? DaysLeft => StartedDate.HasValue
    ? Math.Max(0, EstimatedDaysToComplete - (DateTime.Today - StartedDate.Value.Date).Days)
    : null;
        public DateTime? CompletedDate { get; set; }
        public WorkOrderStatusEnum Status { get; set; }
        public string StatusText => new WorkOrderStatusEnumWrapper().GetDisplayText(Status);
        public List<WorkOrderProgressStageModel>? ProgressStages { get; set; }
        public List<ImageModel>? AdditionalFiles { get; set; }
        public QualityCheckStatusModel? QualityCheckStatus { get; set; }
        public List<WorkOrderQualityCheckRequestModel>? QualityCheckRequests { get; set; }
    }
}
