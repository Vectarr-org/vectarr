using System.Linq.Expressions;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.Classes.Filtering;
using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.DataAccess.IInfrastructure;
using Vectarr.Palooza.DataAccess.Infrastructure;
using Vectarr.Palooza.DataAccess.IRepositories;
using Vectarr.Palooza.Model;

namespace Vectarr.Palooza.DataAccess.Repositories
{
    public class BidRepository : BaseRepository<Bid, BidModel>, IBidRepository, IApplicationRepository<Bid>
    {
        public BidRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public Expression<Func<Bid, bool>> ApplicationPredicate(int applicationId)
        {
            return entity =>
        (entity.Workshop != null && entity.Workshop.UserAccount.Application.Id == applicationId) ||
        (entity.Job.Creator != null && entity.Job.Creator.UserAccount.Application.Id == applicationId);
        }

        public override Expression<Func<Bid, BidModel>> GetEntityToModelExpression()
        {
            return entity => new BidModel()
            {
                Id = entity.Id,
                EstimatedDaysToComplete = entity.EstimatedDaysToComplete,
                Amount = entity.Amount,
                Status = entity.Status,
                Notes = entity.Notes,
                Job = new JobModel() { Id = entity.Job.Id, ManufacturingType = entity.Job.ManufacturingType, Quantity = entity.Job.Quantity, Title = entity.Job.Title , Description = entity.Job.Description },
                Workshop = new WorkshopModel() { Id = entity.Workshop.Id, Name = entity.Workshop.Name },
                DateCreated = entity.CreatedDate,
                Shipping = entity.Shipping,
                Taxes = entity.Taxes,
                Tarrifs = entity.Tarrifs
            };
        }

        public override IQueryable<Bid> Filter(IQueryable<Bid> query, Filter filter)
        {
            switch (filter.FilterVariable)
            {
                case FilterVariables.Status:
                    var status = Enum.Parse<BidStatusEnum>(filter.Value);
                    return query.Where(s => s.Status == status);
            }
            return base.Filter(query, filter);
        }

        protected override Bid ModelToEntityInternal(BidModel model, Bid entity)
        {
            entity.Notes = model.Notes;
            entity.Status = model.Status;
            entity.Amount = model.Amount;
            entity.EstimatedDaysToComplete = model.EstimatedDaysToComplete;
            entity.JobId = model.Job.Id;
            entity.WorkshopId = model.Workshop.Id;
            entity.Shipping = model.Shipping;
            entity.Taxes = model.Taxes;
            entity.Tarrifs = model.Tarrifs;
            return entity;
        }
    }
}
