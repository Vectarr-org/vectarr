using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vectarr.Palooza.Classes.Api
{
    public class BecomeAPartnerRequest
    {
        [Required]
        [MaxLength(200)]
        public string CompanyName { get; set; }

        [Required]
        [MaxLength(200)]
        public string ContactName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(254)]
        public string EmailAddress { get; set; }

        [Required]
        [Phone]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        [Required]
        [MinLength(1)]
        public List<ManufacturingCapability> ManufacturingCapabilities { get; set; } = new();

        [MaxLength(2000)]
        public string? AboutYourShop { get; set; }
    }

    public enum ManufacturingCapability
    {
        CncMachining,
        ThreeDPrinting,
        SheetMetal,
        InjectionMolding
    }
}
