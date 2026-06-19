using Microsoft.AspNetCore.Mvc;
using Vectarr.Palooza.ApiBusiness.IServices;
using Vectarr.Palooza.ApiBusiness.Services.WorkOrderServices;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.DataAccess;

namespace Vectarr.Palooza.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class WorkOrderProgressUpdateController : BaseController<WorkOrderProgressUpdateModel>
    {
        private readonly WorkOrderProgressUpdateServices _workOrderProgressUpdateServices;
        public WorkOrderProgressUpdateController(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, httpContextAccessor)
        {
            _workOrderProgressUpdateServices = new WorkOrderProgressUpdateServices(unitOfWork);
        }

        protected override IBaseManager<WorkOrderProgressUpdateModel> GetManager()
        {
            return _workOrderProgressUpdateServices;
        }
    }
}
