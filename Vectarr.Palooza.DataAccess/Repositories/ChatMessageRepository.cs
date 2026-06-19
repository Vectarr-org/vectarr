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
    public class ChatMessageRepository : BaseRepository<ChatMessage, ChatMessageModel>, IChatMessageRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public ChatMessageRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override Expression<Func<ChatMessage, ChatMessageModel>> GetEntityToModelExpression()
        {
            return entity => new ChatMessageModel
            {
                Id = entity.Id,
                Message = entity.Message,
                DateCreated = entity.CreatedDate,
                ProgressStageId = entity.ProgressStageId,
                CreatorId = entity.CreatorId,
                MessageType = entity.MessageType,
                QuoteRevisionId = entity.QuoteRevisionId,
                WorkOrderId = entity.WorkOrderId,
                WorkshopId = entity.WorkshopId,
            };
        }

        protected override ChatMessage ModelToEntityInternal(ChatMessageModel model, ChatMessage entity)
        {
            entity.Message = model.Message;
            entity.ProgressStageId = model.ProgressStageId;
            entity.CreatorId = model.CreatorId;
            entity.MessageType = model.MessageType;
            entity.QuoteRevisionId = model.QuoteRevisionId;
            entity.WorkOrderId = model.WorkOrderId;
            entity.WorkshopId = model.WorkshopId;
            return entity;
        }
    }
}
