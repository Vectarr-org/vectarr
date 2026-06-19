using Vectarr.Palooza.Classes.BaseApi;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.Classes.Api
{
    public class BidModel : BaseApiModel
    {
        public JobModel Job { get; set; }
        public WorkshopModel Workshop { get; set; }
        public decimal Amount { get; set; }
        public decimal Shipping { get; set; }
        public decimal? Taxes { get; set; }
        public decimal? Tarrifs { get; set; }
        public string? Notes { get; set; }
        public BidStatusEnum Status { get; set; }
        public int? EstimatedDaysToComplete { get; set; }
    }
}
