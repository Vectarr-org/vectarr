using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.DataAccess.Infrastructure;
using Vectarr.Palooza.DataAccess.IRepositories;
using Vectarr.Palooza.Model;
using Vectarr.Palooza.Utils;

namespace Vectarr.Palooza.DataAccess.Repositories
{
    public class WorkOrderProgressStageRepository : BaseRepository<WorkOrderProgressStage, WorkOrderProgressStageModel>, IWorkOrderProgressStageRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public WorkOrderProgressStageRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override Expression<Func<WorkOrderProgressStage, WorkOrderProgressStageModel>> GetEntityToModelExpression()
        {
            var imageExpression = _unitOfWork.Repositories.ImageRepository.GetEntityToModelExpression().Expand();
            var progressUpdatesExpression = _unitOfWork.Repositories.WorkOrderProgressUpdateRepository.GetEntityToModelExpression().Expand();
            return entity => new WorkOrderProgressStageModel()
            {
                Id = entity.Id,
                DateCreated = entity.CreatedDate,
                Stage = entity.Stage,
                WorkshopSubmittedAt = entity.WorkshopSubmittedAt,
                StageImages = entity.StageImages.AsQueryable().Where(w => !w.IsRemoved && !w.IsHidden).Select(imageExpression).ToList(),
                Carrier = entity.Carrier,
                CompletedAt = entity.CompletedAt,
                EstimatedDelivery = entity.EstimatedDelivery,
                Notes = entity.Notes,
                PassedQualityCheck = entity.PassedQualityCheck,
                QualityCheckNotes = entity.QualityCheckNotes,
                TrackingNumber = entity.TrackingNumber,
                Condition = entity.Condition,
                WorkOrderId = entity.WorkOrderId,
                ProgressUpdates = entity.ProgressUpdates.AsQueryable().Where(w => !w.IsRemoved && !w.IsHidden).Select(progressUpdatesExpression).ToList(),
            };
        }

        public override IQueryable<WorkOrderProgressStage> GetIncludes(IQueryable<WorkOrderProgressStage> query)
        {
            return query.Include(w => w.StageImages).Include(w => w.ProgressUpdates);
        }

        protected override WorkOrderProgressStage ModelToEntityInternal(WorkOrderProgressStageModel model, WorkOrderProgressStage entity)
        {
            entity.Stage = model.Stage;

            entity.Carrier = model.Carrier;
            entity.CompletedAt = model.CompletedAt;
            entity.EstimatedDelivery = model.EstimatedDelivery == null ? null : model.EstimatedDelivery.Value.ToUtc();
            entity.WorkshopSubmittedAt = model.WorkshopSubmittedAt;
            entity.Notes = model.Notes;
            entity.PassedQualityCheck = model.PassedQualityCheck;
            entity.QualityCheckNotes = model.QualityCheckNotes;
            entity.TrackingNumber = model.TrackingNumber;
            entity.WorkOrderId = model.WorkOrderId;
            entity.Condition = model.Condition;

            var existingImages = entity.StageImages ?? new List<Image>();
            var updatedImages = new List<Image>();
            if (model.StageImages != null)
            {
                foreach (var image in model.StageImages.ToList())
                {
                    var existingImage = existingImages.FirstOrDefault(x => x.Id == image.Id) ?? _unitOfWork.Repositories.ImageRepository.GetEntityById(image.Id);
                    existingImage.ProgressStageId = entity.Id;
                    updatedImages.Add(existingImage);
                }
            }
            entity.StageImages = _unitOfWork.Repositories.ImageRepository.ReplaceCollection(existingImages, updatedImages);

            var existingProgressUpdates = entity.ProgressUpdates ?? new List<WorkOrderProgressUpdate>();
            var updatedProgressUpdates = new List<WorkOrderProgressUpdate>();
            if (model.ProgressUpdates != null)
            {
                foreach (var progressUpdate in model.ProgressUpdates.ToList())
                {
                    var existingProgressUpdate = existingProgressUpdates.FirstOrDefault(x => x.Id == progressUpdate.Id) ?? new WorkOrderProgressUpdate();
                    existingProgressUpdate = UnitOfWork.Repositories.WorkOrderProgressUpdateRepository.ModelToEntity(progressUpdate, existingProgressUpdate);
                    updatedProgressUpdates.Add(existingProgressUpdate);
                }
            }
            entity.ProgressUpdates = _unitOfWork.Repositories.WorkOrderProgressUpdateRepository.ReplaceCollection(existingProgressUpdates, updatedProgressUpdates);

            return entity;
        }
    }
}
