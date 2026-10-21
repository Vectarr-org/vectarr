using System.Text.Json.Serialization;
using Vectarr.Palooza.Classes.BaseApi;
using Vectarr.Palooza.CoreClasses;
using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.CoreClasses.Wrappers;

namespace Vectarr.Palooza.Classes.Api
{
    public class BidModel : BaseApiModel
    {
        public JobModel Job { get; set; }
        public WorkshopModel Workshop { get; set; }
        /// <summary>Part price.</summary>
        public decimal Amount { get; set; }
        public decimal Shipping { get; set; }
        public decimal? Taxes { get; set; }
        /// <summary>Legacy tariff column. New clients should send <see cref="DutyEstimate"/>. The two are stored in sync.</summary>
        public decimal? Tarrifs { get; set; }

        /// <summary>
        /// Duties and tariff estimate. Defaults to 0, including domestic bids.
        /// This is an estimate for comparison, not a collected duty.
        /// </summary>
        public decimal DutyEstimate { get; set; }

        /// <summary>Part price + shipping + <see cref="DutyEstimate"/>.</summary>
        public decimal LandedCost => BidPricing.LandedCost(Amount, Shipping, DutyEstimate);

        /// <summary>Shipping transit time in days. Production lead time remains <see cref="EstimatedDaysToComplete"/>.</summary>
        public int? TransitDays { get; set; }

        /// <summary>Ship-from country. Omit to default to the shop country. Overridable per bid.</summary>
        public CountryEnum? ShipFromCountry { get; set; }
        public string? ShipFromCountryText => ShipFromCountry.HasValue ? new CountryWrapper().GetDisplayText(ShipFromCountry.Value) : null;

        /// <summary>ISO 3166-1 alpha-2 for <see cref="ShipFromCountry"/>. Writable (for example "CN").</summary>
        public string? ShipFromCountryCode
        {
            get
            {
                if (ShipFromCountry.HasValue)
                {
                    var code = new CountryWrapper().GetCountryCode(ShipFromCountry.Value);
                    if (!string.IsNullOrEmpty(code))
                    {
                        return code;
                    }
                }

                return string.IsNullOrWhiteSpace(ShipFromCountryCodeInput) ? null : ShipFromCountryCodeInput;
            }
            set => ShipFromCountryCodeInput = value;
        }

        [JsonIgnore]
        public string? ShipFromCountryCodeInput { get; private set; }

        public string? Notes { get; set; }
        public BidStatusEnum Status { get; set; }
        public int? EstimatedDaysToComplete { get; set; }
    }
}
