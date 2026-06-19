using System.Linq.Expressions;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.DataAccess.Infrastructure;
using Vectarr.Palooza.DataAccess.IRepositories;
using Vectarr.Palooza.Model;

namespace Vectarr.Palooza.DataAccess.Repositories
{
    public class WorkOrderQualityCheckRejectionRepository : BaseRepository<WorkOrderQualityCheckRejection, WorkOrderQualityCheckRejectionModel>, IWorkOrderQualityCheckRejectionRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public WorkOrderQualityCheckRejectionRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override Expression<Func<WorkOrderQualityCheckRejection, WorkOrderQualityCheckRejectionModel>> GetEntityToModelExpression()
        {
            return entity => new WorkOrderQualityCheckRejectionModel()
            {
                Id = entity.Id,
                DateCreated = entity.CreatedDate,
                RejectionReason = entity.RejectionReason,
                QualityCheckRequestId = entity.QualityCheckRequestId,
            };
        }

        protected override WorkOrderQualityCheckRejection ModelToEntityInternal(WorkOrderQualityCheckRejectionModel model, WorkOrderQualityCheckRejection entity)
        {
            entity.RejectionReason = model.RejectionReason;
            entity.QualityCheckRequestId = model.QualityCheckRequestId;

            return entity;
        }
    }
}
