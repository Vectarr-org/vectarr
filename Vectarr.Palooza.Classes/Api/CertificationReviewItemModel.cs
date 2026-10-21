using Vectarr.Palooza.Classes.BaseApi;
using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.CoreClasses.Wrappers;

namespace Vectarr.Palooza.Classes.Api
{
    /// <summary>
    /// One row of GET api/Workshop/CertificationsForReview.
    /// Review fields match a shop certificate. Workshop name and country are included so an admin can review without a second lookup.
    /// </summary>
    public class CertificationReviewItemModel : BaseApiModel
    {
        public int WorkshopId { get; set; }
        public string? WorkshopName { get; set; }

        /// <summary>Shop country enum. ISO alpha-2 is <see cref="CountryCode"/>.</summary>
        public CountryEnum? Country { get; set; }
        public string? CountryText => Country.HasValue ? new CountryWrapper().GetDisplayText(Country.Value) : null;

        public string? CountryCode
        {
            get
            {
                if (!Country.HasValue)
                {
                    return null;
                }

                var code = new CountryWrapper().GetCountryCode(Country.Value);
                return string.IsNullOrEmpty(code) ? null : code;
            }
        }

        public WorkshopCertificationEnum? Certification { get; set; }
        public string? OtherCertification { get; set; }
        public string? DisplayName => Certification.HasValue ? new WorkshopCertificationEnumWrapper().GetDisplayText(Certification.Value) : null;

        public int? DocumentId { get; set; }
        public DateTime? ExpiryDate { get; set; }

        /// <summary>Pending = 0, Verified = 1, Rejected = 2.</summary>
        public CertificationReviewStatusEnum ReviewStatus { get; set; }
        public string ReviewStatusText => new CertificationReviewStatusEnumWrapper().GetDisplayText(ReviewStatus);
        public int? ReviewedByUserId { get; set; }
        public string? ReviewedByEmail { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? RejectionReason { get; set; }
    }
}
