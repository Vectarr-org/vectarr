using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.ApiBusiness.Services;

namespace Vectarr.Palooza.ApiBusiness.Services.BlogServices
{
    public class BlogServices : BaseManager<BlogModel>
    {
        public BlogServices(IUnitOfWork unitOfWork) : base(unitOfWork, unitOfWork.Repositories.BlogRepository)
        {
        }
    }
}
