using System.Linq.Expressions;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.DataAccess.IInfrastructure;
using Vectarr.Palooza.DataAccess.Infrastructure;
using Vectarr.Palooza.DataAccess.IRepositories;
using Vectarr.Palooza.Model;

namespace Vectarr.Palooza.DataAccess.Repositories
{
    public class WorkshopCustomProgressMessageRepository : BaseRepository<WorkshopCustomProgressMessage, WorkshopCustomProgressMessageModel>, 
        IApplicationRepository<WorkshopCustomProgressMessage>,
        IWorkshopCustomProgressMessageRepository
    {
        public WorkshopCustomProgressMessageRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public Expression<Func<WorkshopCustomProgressMessage, bool>> ApplicationPredicate(int applicationId)
        {
            return entity => entity.Workshop.UserAccount.Application.Id == applicationId;
        }

        public override Expression<Func<WorkshopCustomProgressMessage, WorkshopCustomProgressMessageModel>> GetEntityToModelExpression()
        {
            return entity => new WorkshopCustomProgressMessageModel()
            {
                Id = entity.Id,
                Text = entity.Text,
            };
        }

        protected override WorkshopCustomProgressMessage ModelToEntityInternal(WorkshopCustomProgressMessageModel model, WorkshopCustomProgressMessage entity)
        {
            entity.Text = model.Text;
            entity.WorkshopId = model.WorkShopId;
            return entity;
        }
    }
}
