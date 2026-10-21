using System.Text.Json.Serialization;
using Vectarr.Palooza.Classes.BaseApi;
using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.CoreClasses.Wrappers;

namespace Vectarr.Palooza.Classes.Api
{
    public class WorkshopModel : BaseApiModel
    {
        public string? Name { get; set; }
        public string? WebsiteUrl { get; set; }
        public string? Description { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public ImageModel? Logo { get; set; }
        public List<WorkshopCapabilityModel>? WorkshopCapabilities { get; set; }
        public List<WorkshopSpecializationModel>? WorkshopSpecializations { get; set; }
        public List<WorkshopEquipmentModel>? WorkshopEquipment { get; set; }
        public List<WorkshopCertificationModel>? WorkshopCertifications { get; set; }
        public List<ImageModel>? Images { get; set; }
        public AddressModel? Address { get; set; } // nullable
        public string? CompanyName { get; set; }
        public string? CompanyInfo { get; set; }

        /// <summary>Shop country. Stored as the existing country enum. See <see cref="CountryCode"/> for ISO 3166-1 alpha-2.</summary>
        public CountryEnum? Country { get; set; }
        public string? CountryText => Country.HasValue ? new CountryWrapper().GetDisplayText(Country.Value) : null;

        /// <summary>
        /// ISO 3166-1 alpha-2 (for example "US" or "CN").
        /// On read this is derived from <see cref="Country"/>. On write, a code selects the country when sent.
        /// </summary>
        public string? CountryCode
        {
            get
            {
                if (Country.HasValue)
                {
                    var code = new CountryWrapper().GetCountryCode(Country.Value);
                    if (!string.IsNullOrEmpty(code))
                    {
                        return code;
                    }
                }

                return string.IsNullOrWhiteSpace(CountryCodeInput) ? null : CountryCodeInput;
            }
            set => CountryCodeInput = value;
        }

        [JsonIgnore]
        public string? CountryCodeInput { get; private set; }
    }
}
