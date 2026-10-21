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
        /// <summary>
        /// Duties and tariff estimate used in landed cost. An estimate, not a collected charge.
        /// Domestic bids default to 0. Kept in sync with the legacy <see cref="Tarrifs"/> column.
        /// </summary>
        public decimal DutyEstimate { get; set; }
        /// <summary>Shipping transit time in days. Separate from production lead time.</summary>
        public int? TransitDays { get; set; }
        /// <summary>Country the parts ship from. Defaults to the shop country and can be overridden per bid.</summary>
        public CountryEnum? ShipFromCountry { get; set; }
        public string? Notes { get; set; }
        public BidStatusEnum Status { get; set; }
        public int? EstimatedDaysToComplete { get; set; }
    }
}
