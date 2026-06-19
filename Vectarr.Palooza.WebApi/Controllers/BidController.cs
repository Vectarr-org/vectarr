using Microsoft.AspNetCore.Mvc;
using Vectarr.Palooza.ApiBusiness.IServices;
using Vectarr.Palooza.ApiBusiness.Services.WorkshopServices;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.DataAccess;

namespace Vectarr.Palooza.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class BidController : BaseController<BidModel>
    {
        private readonly BidServices _bidServices;
        public BidController(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, httpContextAccessor)
        {
            _bidServices = new BidServices(unitOfWork);
        }

        protected override IBaseManager<BidModel> GetManager()
        {
            return _bidServices;
        }

        [HttpPost("AcceptBid")]
        public WorkOrderModel AcceptBid(int id)
        {
            return _bidServices.AcceptBid(id);
        }
    }
}
