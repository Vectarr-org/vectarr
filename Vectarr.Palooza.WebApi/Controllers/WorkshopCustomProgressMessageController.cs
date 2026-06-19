using Microsoft.AspNetCore.Mvc;
using Vectarr.Palooza.ApiBusiness.IServices;
using Vectarr.Palooza.ApiBusiness.Services.WorkshopServices;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.DataAccess;

namespace Vectarr.Palooza.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class WorkshopCustomProgressMessageController : BaseController<WorkshopCustomProgressMessageModel>
    {
        private readonly WorkshopCustomProgressMessageServices _workshopCustomProgressMessageServices;
        public WorkshopCustomProgressMessageController(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, httpContextAccessor)
        {
            _workshopCustomProgressMessageServices = new WorkshopCustomProgressMessageServices(unitOfWork);
        }

        protected override IBaseManager<WorkshopCustomProgressMessageModel> GetManager()
        {
            return _workshopCustomProgressMessageServices;
        }
    }
}
