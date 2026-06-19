using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.Classes.BaseApi;

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
    }
}
