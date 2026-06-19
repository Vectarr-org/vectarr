using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.Model;
using Vectarr.Palooza.DataAccess.Infrastructure;
using Vectarr.Palooza.DataAccess.IRepositories;

namespace Vectarr.Palooza.DataAccess.Repositories
{
    public class WorkshopSpecializationRepository : BaseRepository<WorkshopSpecialization, WorkshopSpecializationModel>, IWorkshopSpecializationRepository
    {
        public WorkshopSpecializationRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public override Expression<Func<WorkshopSpecialization, WorkshopSpecializationModel>> GetEntityToModelExpression()
        {
            return entity => new WorkshopSpecializationModel()
            {
                Id = entity.Id,
                Specialization = entity.Specialization,
                OtherSpecialization = entity.OtherSpecialization,
            };
        }

        protected override WorkshopSpecialization ModelToEntityInternal(WorkshopSpecializationModel model, WorkshopSpecialization entity)
        {
            entity.Specialization = model.Specialization;
            entity.OtherSpecialization = model.OtherSpecialization;
            return entity;
        }
    }
}
