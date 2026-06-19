using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.CoreClasses.Wrappers;
using Vectarr.Palooza.Classes.BaseApi;

namespace Vectarr.Palooza.Classes.Api
{
    public class AddressModel : BaseApiModel
    {
        public CountryEnum? Country { get; set; }
        public string? Street1 { get; set; }
        public string? Street2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string CountryText => Country.HasValue ? new CountryWrapper().GetDisplayText(Country.Value) : "";
        public string CountryCode => Country.HasValue ? new CountryWrapper().GetCountryCode(Country.Value) : "";
    }
}
