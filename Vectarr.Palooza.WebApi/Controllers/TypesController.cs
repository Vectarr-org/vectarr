using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vectarr.Palooza.CoreClasses;
using Vectarr.Palooza.CoreClasses.Wrappers;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.Model;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class TypesController : SystemController
    {
        public TypesController(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, httpContextAccessor)
        {
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("Countries")]
        public List<EnumListModel> Countries()
        {
            return new CountryWrapper().GetNonOrderedListModel();
        }

        [HttpGet]
        [Route("Conditions")]
        public List<EnumListModel> Conditions()
        {
            return new ConditionEnumWrapper().GetNonOrderedListModel();
        }

        [HttpGet]
        [Route("JobStatuses")]
        public List<EnumListModel> JobStatuses()
        {
            return new JobStatusEnumWrapper().GetNonOrderedListModel();
        }

        [HttpGet]
        [Route("ManufacturingTypes")]
        [AllowAnonymous]
        public List<EnumListModel> ManufacturingTypes()
        {
            return new ManufacturingTypeEnumWrapper().GetNonOrderedListModel();
        }

        [HttpGet]
        [Route("Tolerances")]
        [AllowAnonymous]
        public List<EnumListModel> Tolerances()
        {
            return new ToleranceEnumWrapper().GetNonOrderedListModel();
        }

        [HttpGet]
        [Route("Materials")]
        [AllowAnonymous]
        public List<EnumListModel> Materials()
        {
            return new MaterialEnumWrapper().GetNonOrderedListModel();
        }

        [HttpGet]
        [Route("SourceFinishes")]
        [AllowAnonymous]
        public List<EnumListModel> SourceFinishes()
        {
            return new SourceFinishEnumWrapper().GetNonOrderedListModel();
        }

        [HttpGet]
        [Route("WorkshopEquipment")]
        public List<EnumListModel> WorkshopEquipment()
        {
            return new WorkshopEquipmentEnumWrapper().GetNonOrderedListModel();
        }

        [HttpGet]
        [Route("GetCostVsSpeed")]
        [AllowAnonymous]
        public List<EnumListModel> GetCostVsSpeed()
        {
            var values = Enum.GetValues(typeof(CostVsSpeedEnum));
            return values.Cast<CostVsSpeedEnum>().ToList().Select(x => new EnumListModel
            {
                Id = Convert.ToInt32(x),
                DisplayName = new CostVsSpeedEnumWrapper().GetDisplayText(x),
                Description = new CostVsSpeedEnumWrapper().GetDisplayDescription(x)
            })
            .ToList();
        }

        [HttpGet]
        [Route("QualityCheckTypes")]
        public List<EnumListModel> QualityCheckTypes()
        {
            var values = Enum.GetValues(typeof(QualityCheckTypeEnum));
            return values.Cast<QualityCheckTypeEnum>().ToList().Select(x => new EnumListModel
            {
                Id = Convert.ToInt32(x),
                DisplayName = new QualityCheckTypeEnumWrapper().GetDisplayText(x),
                Description = new QualityCheckTypeEnumWrapper().GetDisplayDescription(x)
            })
            .ToList();
        }

        [HttpGet]
        [Route("WorkshopSpecializations")]
        public List<EnumListModel> WorkshopSpecializations()
        {
            return new WorkshopSpecializationEnumWrapper().GetNonOrderedListModel();
        }

        [HttpGet]
        [Route("WorkshopCapabilities")]
        public List<EnumListModel> WorkshopCapabilities()
        {
            return new ManufacturingTypeEnumWrapper().GetNonOrderedListModel();
        }

        [HttpGet]
        [Route("WorkshopCertifications")]
        public List<EnumListModel> WorkshopCertifications()
        {
            return new WorkshopCertificationEnumWrapper().GetNonOrderedListModel();
        }

    }
}
