using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.Model
{
    [Table("Job.ChatMessages")]
    public class ChatMessage : BaseModel
    {
        public int WorkOrderId { get; set; }
        public virtual WorkOrder WorkOrder { get; set; }

        public MessageTypeEnum MessageType { get; set; }

        public int? CreatorId { get; set; }
        public virtual Creator? Creator { get; set; }

        public int? WorkshopId { get; set; }
        public virtual Workshop? Workshop { get; set; }

        public string? Message { get; set; }

        public int? QuoteRevisionId { get; set; }
        public virtual QuoteRevision? QuoteRevision { get; set; }

        public int? ProgressStageId { get; set; }
        public virtual WorkOrderProgressStage? ProgressStage { get; set; }
    }
}
