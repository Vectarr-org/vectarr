using System.Linq.Expressions;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.Model;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.DataAccess.Infrastructure;
using Vectarr.Palooza.DataAccess.IRepositories;

namespace Vectarr.Palooza.DataAccess.Repositories
{
    public class WorkshopCertificationRepository : BaseRepository<WorkshopCertification, WorkshopCertificationModel>, IWorkshopCertificationRepository
    {
        public WorkshopCertificationRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public override Expression<Func<WorkshopCertification, WorkshopCertificationModel>> GetEntityToModelExpression()
        {
            return entity => new WorkshopCertificationModel()
            {
                Id = entity.Id,
                Certification = entity.Certification,
                OtherCertification = entity.OtherCertification,
            };
        }

        protected override WorkshopCertification ModelToEntityInternal(WorkshopCertificationModel model, WorkshopCertification entity)
        {
            entity.Certification = model.Certification;
            entity.OtherCertification = model.OtherCertification;
            return entity;
        }
    }
}
