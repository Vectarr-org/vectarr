using System.Linq.Expressions;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.DataAccess.Infrastructure;
using Vectarr.Palooza.DataAccess.IRepositories;
using Vectarr.Palooza.Model;

namespace Vectarr.Palooza.DataAccess.Repositories
{
    public class JobQualityCheckRequestRepository : BaseRepository<JobQualityCheckRequest, JobQualityCheckRequestModel>, IJobQualityCheckRequestRepository
    {
        public JobQualityCheckRequestRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public override Expression<Func<JobQualityCheckRequest, JobQualityCheckRequestModel>> GetEntityToModelExpression()
        {
            return entity => new JobQualityCheckRequestModel()
            {
                Id = entity.Id,
                CheckType = entity.CheckType,
                Instructions = entity.Instructions,
                JobId = entity.JobId,
            };
        }

        protected override JobQualityCheckRequest ModelToEntityInternal(JobQualityCheckRequestModel model, JobQualityCheckRequest entity)
        {
            entity.CheckType = model.CheckType;
            entity.Instructions = model.Instructions;
            entity.JobId = model.JobId;
            return entity;
        }
    }
}
