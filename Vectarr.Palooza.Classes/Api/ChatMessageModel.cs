using Vectarr.Palooza.Classes.BaseApi;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.Classes.Api
{
    public class ChatMessageModel : BaseApiModel
    {
        public int WorkOrderId { get; set; }
        public MessageTypeEnum MessageType { get; set; }
        public int? CreatorId { get; set; }
        public int? WorkshopId { get; set; }
        public string? Message { get; set; }
        public int? QuoteRevisionId { get; set; }
        public int? ProgressStageId { get; set; }
    }
}
