using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.DataAccess.IInfrastructure;

namespace Vectarr.Palooza.ApiBusiness.Services.WorkshopServices
{
    public class WorkshopCustomProgressMessageServices : BaseManager<WorkshopCustomProgressMessageModel>
    {
        public WorkshopCustomProgressMessageServices(IUnitOfWork unitOfWork) : base(unitOfWork, unitOfWork.Repositories.WorkshopCustomProgressMessageRepository)
        {
        }
    }
}
