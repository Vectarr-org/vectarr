using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.Classes.Api
{
    /// <summary>
    /// Admin review of a shop certificate. Shops cannot set review fields themselves.
    /// POST api/Workshop/ReviewCertification
    /// </summary>
    public class CertificationReviewRequest
    {
        public int CertificationId { get; set; }
        public CertificationReviewStatusEnum ReviewStatus { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
