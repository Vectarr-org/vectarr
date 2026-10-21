using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.CoreClasses.Wrappers;
using Vectarr.Palooza.Classes.BaseApi;

namespace Vectarr.Palooza.Classes.Api
{
    public class WorkshopCertificationModel : BaseApiModel
    {
        public WorkshopCertificationEnum? Certification { get; set; }
        public string? OtherCertification { get; set; }
        public string? DisplayName => Certification.HasValue ? new WorkshopCertificationEnumWrapper().GetDisplayText(Certification.Value) : null;

        /// <summary>Certificate file previously uploaded via api/File/UploadImage.</summary>
        public int? DocumentId { get; set; }
        public ImageModel? Document { get; set; }

        /// <summary>Pending = 0, Verified = 1, Rejected = 2. Shops cannot set this; new uploads are Pending.</summary>
        public CertificationReviewStatusEnum ReviewStatus { get; set; }
        public string ReviewStatusText => new CertificationReviewStatusEnumWrapper().GetDisplayText(ReviewStatus);

        public int? ReviewedByUserId { get; set; }
        public string? ReviewedByEmail { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? RejectionReason { get; set; }
    }
}
