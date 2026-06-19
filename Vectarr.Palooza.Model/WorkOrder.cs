using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.Model
{
    [Table("Job.WorkOrders")]
    public class WorkOrder : BaseModel
    {
        public int JobId { get; set; }
        public virtual Job Job { get; set; }

        public int AcceptedBidId { get; set; }
        public virtual Bid AcceptedBid { get; set; }

        public int CreatorId { get; set; }
        public virtual Creator Creator { get; set; }

        public int WorkshopId { get; set; }
        public virtual Workshop Workshop { get; set; }

        public decimal CurrentPrice { get; set; }
        public DateTime? CurrentDeadline { get; set; }
        public DateTime? StartedDate { get; set; }
        public DateTime? CompletedDate { get; set; }

        public WorkOrderStatusEnum Status { get; set; }

        public virtual ICollection<ChatMessage> ChatMessages { get; set; }
        public virtual ICollection<QuoteRevision> QuoteRevisions { get; set; }
        public virtual ICollection<WorkOrderProgressStage> ProgressStages { get; set; }
        public virtual ICollection<Image> AdditionalFiles { get; set; }
        public virtual ICollection<WorkOrderQualityCheckRequest> QualityCheckRequests { get; set; }
    }
}
