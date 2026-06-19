using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.ApiBusiness.Services;

namespace Vectarr.Palooza.ApiBusiness.Services.UserServices
{
    public class IdentityServices : BaseManager<IdentityModel>
    {
        private readonly IUnitOfWork _unitOfWork;
        public IdentityServices(IUnitOfWork unitOfWork) : base(unitOfWork, unitOfWork.Repositories.IdentityRepository)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
