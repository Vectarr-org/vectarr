using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.DataAccess.Infrastructure;
using Vectarr.Palooza.DataAccess.IRepositories;
using Vectarr.Palooza.Model;

namespace Vectarr.Palooza.DataAccess.Repositories
{
    public class QuoteRevisionRepository : BaseRepository<QuoteRevision, QuoteRevisionModel>, IQuoteRevisionRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public QuoteRevisionRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override Expression<Func<QuoteRevision, QuoteRevisionModel>> GetEntityToModelExpression()
        {
            return entity => new QuoteRevisionModel()
            {
                Id = entity.Id,
                DateCreated = entity.CreatedDate,
                Status = entity.Status,
                ChangeSummary = entity.ChangeSummary,
                RespondedAt = entity.RespondedAt,
                NewDeadline = entity.NewDeadline,
                NewPrice = entity.NewPrice,
                OldDeadline = entity.OldDeadline,
                OldPrice = entity.OldPrice,
                Reason = entity.Reason,
                RespondedByCreatorId = entity.RespondedByCreatorId,
                WorkOrderId = entity.WorkOrderId,

            };
        }

        protected override QuoteRevision ModelToEntityInternal(QuoteRevisionModel model, QuoteRevision entity)
        {
            entity.Status = model.Status;
            entity.ChangeSummary = model.ChangeSummary;
            entity.RespondedAt = model.RespondedAt;
            entity.NewDeadline = model.NewDeadline;
            entity.NewPrice = model.NewPrice;
            entity.OldDeadline = model.OldDeadline;
            entity.OldPrice = model.OldPrice;
            entity.Reason = model.Reason;
            entity.RespondedByCreatorId = model.RespondedByCreatorId;
            entity.WorkOrderId = model.WorkOrderId;
            return entity;
        }
    }
}
