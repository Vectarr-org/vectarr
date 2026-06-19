using System.Text.Json.Serialization;
using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.Classes.BaseApi;

namespace Vectarr.Palooza.Classes.Api
{
    public class IdentityModel : BaseApiModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? EmailAddress { get; set; }
        public string? PhoneNumber { get; set; }
        public AddressModel? Address { get; set; }
        public ImageModel? ProfilePicture { get; set; }
        public int? Age { get; set; }
        public string? referralCode { get; set; }
        public bool IsEmailSubscribed { get; set; }
        public decimal? TaxRate { get; set; }
        public string DisplayName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(FirstName) && !string.IsNullOrWhiteSpace(LastName))
                    return $"{FirstName} {LastName}";
                else if (!string.IsNullOrWhiteSpace(FirstName))
                    return FirstName;
                else if (!string.IsNullOrWhiteSpace(LastName))
                    return LastName;
                else
                    return EmailAddress ?? string.Empty;
            }
        }
    }
}
