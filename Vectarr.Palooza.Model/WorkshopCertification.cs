using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.Model
{
    [Table("WorkshopCertification.WorkshopCertifications")]
    public class WorkshopCertification : BaseModel
    {
        public int WorkshopId { get; set; }
        public virtual Workshop Workshop { get; set; }
        public WorkshopCertificationEnum? Certification { get; set; }
        public string? OtherCertification { get; set; }

        /// <summary>Uploaded certificate file. Shops upload through api/File/UploadImage and pass the image id.</summary>
        public int? DocumentId { get; set; }
        public virtual Image? Document { get; set; }

        public CertificationReviewStatusEnum ReviewStatus { get; set; }
        public int? ReviewedByUserId { get; set; }
        public virtual UserAccount? ReviewedBy { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public DateTime? ExpiryDate { get; set; }

        [StringLength(2000)]
        public string? RejectionReason { get; set; }
    }
}
