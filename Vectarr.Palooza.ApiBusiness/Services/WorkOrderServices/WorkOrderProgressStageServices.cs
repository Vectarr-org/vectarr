using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.DataAccess.IInfrastructure;
using Vectarr.Palooza.Model;

namespace Vectarr.Palooza.ApiBusiness.Services.WorkOrderServices
{
    public class WorkOrderProgressStageServices : BaseManager<WorkOrderProgressStageModel>
    {
        public WorkOrderProgressStageServices(IUnitOfWork unitOfWork) : base(unitOfWork, unitOfWork.Repositories.WorkOrderProgressStageRepository)
        {
        }
    }
}
