using System.Linq.Expressions;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.Model;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.DataAccess.Infrastructure;
using Vectarr.Palooza.DataAccess.IRepositories;

namespace Vectarr.Palooza.DataAccess.Repositories
{
    public class WorkshopEquipmentRepository : BaseRepository<WorkshopEquipment, WorkshopEquipmentModel>, IWorkshopEquipmentRepository
    {
        public WorkshopEquipmentRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public override Expression<Func<WorkshopEquipment, WorkshopEquipmentModel>> GetEntityToModelExpression()
        {
            return entity => new WorkshopEquipmentModel()
            {
                Id = entity.Id,
                Equipment = entity.Equipment,
                OtherEquipment = entity.OtherEquipment,
            };
        }

        protected override WorkshopEquipment ModelToEntityInternal(WorkshopEquipmentModel model, WorkshopEquipment entity)
        {
            entity.Equipment = model.Equipment;
            entity.OtherEquipment = model.OtherEquipment;
            return entity;
        }
    }
}
