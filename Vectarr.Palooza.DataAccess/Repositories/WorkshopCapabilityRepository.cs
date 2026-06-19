using System.Linq.Expressions;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.Model;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.DataAccess.Infrastructure;
using Vectarr.Palooza.DataAccess.IRepositories;

namespace Vectarr.Palooza.DataAccess.Repositories
{
    public class WorkshopCapabilityRepository : BaseRepository<WorkshopCapability, WorkshopCapabilityModel>, IWorkshopCapabilityRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public WorkshopCapabilityRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override Expression<Func<WorkshopCapability, WorkshopCapabilityModel>> GetEntityToModelExpression()
        {
            return entity => new WorkshopCapabilityModel()
            {
                Id = entity.Id,
                Capability = entity.Capability,
                OtherCapability = entity.OtherCapability,
            };
        }

        protected override WorkshopCapability ModelToEntityInternal(WorkshopCapabilityModel model, WorkshopCapability entity)
        {
            entity.Capability = model.Capability;
            entity.OtherCapability = model.OtherCapability;
            return entity;
        }
    }
}
