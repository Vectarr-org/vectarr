using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.Model
{
    [Table("Workshop.Workshops")]
    public class Workshop : BaseModel
    {
        public int UserAccountId { get; set; }
        public virtual UserAccount UserAccount { get; set; }

        public string? Name { get; set; }
        public string? WebsiteUrl { get; set; }
        public string? Description { get; set; }
        public string? PhoneNumber { get; set; }
        public string Email { get; set; }
        public Image? Logo { get; set; }
        public int? LogoId { get; set; }
        public virtual ICollection<WorkshopCapability> WorkshopCapabilities { get; set; }
        public virtual ICollection<WorkshopSpecialization> WorkshopSpecializations { get; set; }
        public virtual ICollection<WorkshopEquipment> WorkshopEquipment { get; set; }
        public virtual ICollection<WorkshopCertification> WorkshopCertifications { get; set; }
        public virtual ICollection<Image> Images { get; set; }
        public int? AddressId { get; set; } // nullable
        public virtual Address? Address { get; set; } // nullable
        /// <summary>
        /// Shop country. ISO 3166-1 alpha-2 is exposed as CountryCode on the API model.
        /// Kept in sync with <see cref="Address"/> when either side is set.
        /// </summary>
        public CountryEnum? Country { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyInfo { get; set; }
        public virtual ICollection<Bid> Bids { get; set; }
        public virtual ICollection<WorkOrder> WorkOrders { get; set; }
    }
}
