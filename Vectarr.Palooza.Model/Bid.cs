using System.ComponentModel.DataAnnotations.Schema;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.Model
{
    [Table("Job.Bids")]
    public class Bid : BaseModel
    {
        public int JobId { get; set; }
        public virtual Job Job { get; set; }
        public int WorkshopId { get; set; }
        public virtual Workshop Workshop { get; set; }
        public decimal Amount { get; set; }
        public decimal Shipping { get; set; }
        public decimal? Taxes { get; set; }
        public decimal? Tarrifs { get; set; }
        public string? Notes { get; set; }
        public BidStatusEnum Status { get; set; }
        public int? EstimatedDaysToComplete { get; set; }
    }
}
