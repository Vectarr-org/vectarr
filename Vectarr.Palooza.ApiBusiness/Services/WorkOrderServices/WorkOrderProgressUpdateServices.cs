using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.DataAccess;

namespace Vectarr.Palooza.ApiBusiness.Services.WorkOrderServices
{
    public class WorkOrderProgressUpdateServices : BaseManager<WorkOrderProgressUpdateModel>
    {
        public WorkOrderProgressUpdateServices(IUnitOfWork unitOfWork) : base(unitOfWork, unitOfWork.Repositories.WorkOrderProgressUpdateRepository)
        {
        }
    }
}
