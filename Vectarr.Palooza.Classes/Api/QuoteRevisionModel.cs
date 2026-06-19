using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.Classes.BaseApi;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.Classes.Api
{
    public class QuoteRevisionModel : BaseApiModel
    {
        public int WorkOrderId { get; set; }
        public decimal NewPrice { get; set; }
        public decimal OldPrice { get; set; }
        public DateTime? NewDeadline { get; set; }
        public DateTime? OldDeadline { get; set; }
        public string? Reason { get; set; }
        public string? ChangeSummary { get; set; }
        public RevisionStatusEnum Status { get; set; }
        public DateTime? RespondedAt { get; set; }
        public int? RespondedByCreatorId { get; set; }
    }
}
