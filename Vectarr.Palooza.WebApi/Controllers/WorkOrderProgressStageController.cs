using Microsoft.AspNetCore.Mvc;
using Vectarr.Palooza.ApiBusiness.IServices;
using Vectarr.Palooza.ApiBusiness.Services.WorkOrderServices;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.Model;

namespace Vectarr.Palooza.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class WorkOrderProgressStageController : BaseController<WorkOrderProgressStageModel>
    {
        private readonly WorkOrderProgressStageServices _workOrderProgressStageServices;
        public WorkOrderProgressStageController(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, httpContextAccessor)
        {
            _workOrderProgressStageServices = new WorkOrderProgressStageServices(unitOfWork);
        }

        protected override IBaseManager<WorkOrderProgressStageModel> GetManager()
        {
            return _workOrderProgressStageServices;
        }
    }
}
