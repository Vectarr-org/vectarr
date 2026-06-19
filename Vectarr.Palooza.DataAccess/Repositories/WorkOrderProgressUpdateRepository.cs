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

namespace Vectarr.Palooza.DataAccess.Repositories
{
    public class WorkOrderProgressUpdateRepository : BaseRepository<WorkOrderProgressUpdate, WorkOrderProgressUpdateModel>, IWorkOrderProgressUpdateRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public WorkOrderProgressUpdateRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override Expression<Func<WorkOrderProgressUpdate, WorkOrderProgressUpdateModel>> GetEntityToModelExpression()
        {
            var imageExpression = _unitOfWork.Repositories.ImageRepository.GetEntityToModelExpression().Expand();
            return entity => new WorkOrderProgressUpdateModel()
            {
                Id = entity.Id,
                DateCreated = entity.CreatedDate,
                ProgressStageId = entity.ProgressStageId,
                Notes = entity.Notes,
                PercentComplete = entity.PercentComplete,
                WorkshopCustomProgressMessageId = entity.WorkshopCustomProgressMessage != null ? entity.WorkshopCustomProgressMessage.Id : null,
                WorkshopCustomProgressMessageText = entity.WorkshopCustomProgressMessage != null ? entity.WorkshopCustomProgressMessage.Text : null,
                Images = entity.Images.AsQueryable().Where(w => !w.IsRemoved && !w.IsHidden).Select(imageExpression).ToList(),
            };
        }

        public override IQueryable<WorkOrderProgressUpdate> GetIncludes(IQueryable<WorkOrderProgressUpdate> query)
        {
            return query.Include(w => w.Images);
        }

        protected override WorkOrderProgressUpdate ModelToEntityInternal(WorkOrderProgressUpdateModel model, WorkOrderProgressUpdate entity)
        {
            entity.ProgressStageId = model.ProgressStageId;
            entity.Notes = model.Notes;
            entity.PercentComplete = model.PercentComplete;
            entity.WorkshopCustomProgressMessageId = model.WorkshopCustomProgressMessageId;

            var existingImages = entity.Images ?? new List<Image>();
            var updatedImages = new List<Image>();
            if (model.Images != null)
            {
                foreach (var image in model.Images.ToList())
                {
                    var existingImage = existingImages.FirstOrDefault(x => x.Id == image.Id) ?? _unitOfWork.Repositories.ImageRepository.GetEntityById(image.Id);
                    existingImage.ProgressUpdateId = entity.Id;
                    updatedImages.Add(existingImage);
                }
            }
            entity.Images = _unitOfWork.Repositories.ImageRepository.ReplaceCollection(existingImages, updatedImages);

            return entity;
        }
    }
}
