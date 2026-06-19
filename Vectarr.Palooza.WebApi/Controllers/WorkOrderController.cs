using Microsoft.AspNetCore.Mvc;
using Vectarr.Palooza.ApiBusiness.IServices;
using Vectarr.Palooza.ApiBusiness.Services.WorkOrderServices;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.DataAccess;

namespace Vectarr.Palooza.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class WorkOrderController : BaseController<WorkOrderModel>
    {
        private readonly WorkOrderServices _workOrderServices;
        public WorkOrderController(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, httpContextAccessor)
        {
            _workOrderServices = new WorkOrderServices(unitOfWork);
        }

        protected override IBaseManager<WorkOrderModel> GetManager()
        {
            return _workOrderServices;
        }

        [HttpPost("UploadAdditionalFiles")]
        public WorkOrderModel UploadAdditionalFiles(int workOrderId, List<ImageModel> files)
        {
            return _workOrderServices.UploadAdditionalFiles(workOrderId, files);
        }
    }
}
