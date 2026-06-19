using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.CoreClasses.Wrappers;
using Vectarr.Palooza.Classes.BaseApi;

namespace Vectarr.Palooza.Classes.Api
{
    public class WorkshopCapabilityModel : BaseApiModel
    {
        public ManufacturingTypeEnum? Capability { get; set; }
        public string? OtherCapability { get; set; }
        public string? DisplayName => Capability.HasValue ? new ManufacturingTypeEnumWrapper().GetDisplayText(Capability.Value) : null;
    }
}
