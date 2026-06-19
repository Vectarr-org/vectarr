using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.DataAccess.IInfrastructure;
using Vectarr.Palooza.ApiBusiness.Services;

namespace Vectarr.Palooza.ApiBusiness.Services.CreatorServices
{
    public class CreatorServices : BaseManager<CreatorModel>
    {
        public CreatorServices(IUnitOfWork unitOfWork) : base(unitOfWork, unitOfWork.Repositories.CreatorRepository)
        {
        }
    }
}
