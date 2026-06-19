using System.Linq.Expressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.DataAccess.Infrastructure;
using Vectarr.Palooza.DataAccess.IRepositories;
using Vectarr.Palooza.Model;

namespace Vectarr.Palooza.DataAccess.Repositories
{
    public class WorkOrderQualityCheckRequestRepository : BaseRepository<WorkOrderQualityCheckRequest, WorkOrderQualityCheckRequestModel>, IWorkOrderQualityCheckRequestRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public WorkOrderQualityCheckRequestRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override Expression<Func<WorkOrderQualityCheckRequest, WorkOrderQualityCheckRequestModel>> GetEntityToModelExpression()
        {
            var imageExpression = _unitOfWork.Repositories.ImageRepository.GetEntityToModelExpression().Expand();
            return entity => new WorkOrderQualityCheckRequestModel()
            {
                Id = entity.Id,
                WorkOrderId = entity.WorkOrderId,
                CheckType = entity.CheckType,
                CompletedAt = entity.CompletedAt,
                Instructions = entity.Instructions,
                Passed = entity.Passed,
                ResultNotes = entity.ResultNotes,
                RejectedCount   = entity.Rejections.Count(w => !w.IsRemoved && !w.IsHidden),
                Attachments = entity.Attachments.AsQueryable().Where(w => !w.IsRemoved && !w.IsHidden).Select(imageExpression).ToList(),
            };
        }

        public override IQueryable<WorkOrderQualityCheckRequest> GetIncludes(IQueryable<WorkOrderQualityCheckRequest> query)
        {
            return query.Include(s => s.Attachments);
        }

        protected override WorkOrderQualityCheckRequest ModelToEntityInternal(WorkOrderQualityCheckRequestModel model, WorkOrderQualityCheckRequest entity)
        {
            entity.WorkOrderId = model.WorkOrderId;
            entity.CheckType = model.CheckType;
            entity.CompletedAt  = model.CompletedAt;
            entity.Instructions = model.Instructions;
            entity.Passed = model.Passed;
            entity.ResultNotes = model.ResultNotes;
            var existingImages = entity.Attachments ?? new List<Image>();
            var updatedImages = new List<Image>();
            if (model.Attachments != null)
            {
                foreach (var image in model.Attachments.ToList())
                {
                    var existingImage = existingImages.FirstOrDefault(x => x.Id == image.Id) ?? _unitOfWork.Repositories.ImageRepository.GetEntityById(image.Id);
                    existingImage.WorkOrderQualityCheckRequestId = entity.Id;
                    updatedImages.Add(existingImage);
                }
            }
            entity.Attachments = _unitOfWork.Repositories.ImageRepository.ReplaceCollection(existingImages, updatedImages);

            return entity;
        }
    }
}
