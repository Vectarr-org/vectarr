using System.Threading.Tasks;
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
    }
}
