using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.Classes.Filtering;
using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.DataAccess.Infrastructure;
using Vectarr.Palooza.DataAccess.IRepositories;
using Vectarr.Palooza.Model;

namespace Vectarr.Palooza.DataAccess.Repositories
{
    public class WorkOrderRepository : BaseRepository<WorkOrder, WorkOrderModel>, IWorkOrderRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public WorkOrderRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override Expression<Func<WorkOrder, WorkOrderModel>> GetEntityToModelExpression()
        {
            var imageExpression = _unitOfWork.Repositories.ImageRepository.GetEntityToModelExpression().Expand();
            var progressStageExpression = _unitOfWork.Repositories.WorkOrderProgressStageRepository.GetEntityToModelExpression().Expand();
            var workOrderQualityCheckRequestExpression = _unitOfWork.Repositories.WorkOrderQualityCheckRequestRepository.GetEntityToModelExpression().Expand();
            return entity => new WorkOrderModel()
            {
                Id = entity.Id,
                DateCreated = entity.CreatedDate,
                StartedDate = entity.StartedDate,
                CompletedDate = entity.CompletedDate,
                Status = entity.Status,
                EstimatedDaysToComplete = entity.AcceptedBid.EstimatedDaysToComplete != null ? entity.AcceptedBid.EstimatedDaysToComplete.Value : 0,
                Job = new JobModel()
                {
                    Id = entity.Job.Id,
                    ManufacturingType = entity.Job.ManufacturingType,
                    Quantity = entity.Job.Quantity,
                    Title = entity.Job.Title,
                    Status = entity.Job.Status,
                    Description = entity.Job.Description,
                    Material = entity.Job.Material,
                    Files = entity.Job.Files.AsQueryable().Where(w => !w.IsRemoved && !w.IsHidden).Select(imageExpression).ToList(),
                    QCPercentage = entity.Job.QCPrecentage,
                    RequireQcConfirmation = entity.Job.RequireQcConfirmation,
                    AutoAcceptQcReport = entity.Job.AutoAcceptQcReport
                },
                JobId = entity.JobId,
                WorkshopId = entity.WorkshopId,
                CreatorId = entity.CreatorId,
                CurrentDeadline = entity.CurrentDeadline,
                AcceptedBidId = entity.AcceptedBidId,
                AcceptedBid = new BidModel()
                {
                    Id = entity.AcceptedBid.Id,
                    EstimatedDaysToComplete = entity.AcceptedBid.EstimatedDaysToComplete,
                    Amount = entity.AcceptedBid.Amount,
                    Status = entity.AcceptedBid.Status,
                    Notes = entity.AcceptedBid.Notes,
                    Shipping = entity.AcceptedBid.Shipping,
                    Taxes = entity.AcceptedBid.Taxes,
                    Tarrifs = entity.AcceptedBid.Tarrifs,
                    DutyEstimate = entity.AcceptedBid.DutyEstimate != 0 ? entity.AcceptedBid.DutyEstimate : (entity.AcceptedBid.Tarrifs ?? 0m),
                    TransitDays = entity.AcceptedBid.TransitDays,
                    ShipFromCountry = entity.AcceptedBid.ShipFromCountry
                },
                CurrentPrice = entity.CurrentPrice,
                AdditionalFiles = entity.AdditionalFiles.AsQueryable().Where(w => !w.IsRemoved && !w.IsHidden).Select(imageExpression).ToList(),
                ProgressStages = entity.ProgressStages.AsQueryable().Where(w => !w.IsRemoved && !w.IsHidden).Select(progressStageExpression).ToList(),
                QualityCheckRequests = entity.QualityCheckRequests.AsQueryable().Where(w => !w.IsRemoved && !w.IsHidden).Select(workOrderQualityCheckRequestExpression).ToList()
            };
        }

        public override IQueryable<WorkOrder> GetIncludes(IQueryable<WorkOrder> query)
        {
            return query.Include(w => w.AdditionalFiles).Include(w => w.ProgressStages).ThenInclude(s => s.ProgressUpdates);
        }

        public override IQueryable<WorkOrder> Filter(IQueryable<WorkOrder> query, Filter filter)
        {
            switch (filter.FilterVariable)
            {
                case FilterVariables.Status:
                    var statusStrings = filter.Value.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    var statuses = statusStrings
                        .Select(s => Enum.Parse<WorkOrderStatusEnum>(s.Trim()))
                        .ToList();

                    return query.Where(s => statuses.Contains(s.Status));
            }
            return base.Filter(query, filter);
        }

        protected override WorkOrder ModelToEntityInternal(WorkOrderModel model, WorkOrder entity)
        {
            entity.StartedDate = model.StartedDate;
            entity.CompletedDate = model.CompletedDate;
            entity.Status = model.Status;
            entity.JobId = model.JobId;
            entity.WorkshopId = model.WorkshopId;
            entity.CreatorId = model.CreatorId;
            entity.CurrentDeadline = model.CurrentDeadline;
            entity.AcceptedBidId = model.AcceptedBidId;
            entity.CurrentPrice = model.CurrentPrice;

            var existingImages = entity.AdditionalFiles ?? new List<Image>();
            var updatedImages = new List<Image>();
            if (model.AdditionalFiles != null)
            {
                foreach (var image in model.AdditionalFiles.ToList())
                {
                    var existingImage = existingImages.FirstOrDefault(x => x.Id == image.Id) ?? _unitOfWork.Repositories.ImageRepository.GetEntityById(image.Id);
                    existingImage.WorkOrderId = entity.Id;
                    updatedImages.Add(existingImage);
                }
            }
            entity.AdditionalFiles = _unitOfWork.Repositories.ImageRepository.ReplaceCollection(existingImages, updatedImages);

            var existingProgressStages = entity.ProgressStages ?? new List<WorkOrderProgressStage>();
            var updatedProgressStages = new List<WorkOrderProgressStage>();
            if (model.ProgressStages != null)
            {
                foreach (var progressStage in model.ProgressStages.ToList())
                {
                    var existingStage = existingProgressStages.FirstOrDefault(x => x.Id == progressStage.Id) ?? new WorkOrderProgressStage();
                    existingStage = UnitOfWork.Repositories.WorkOrderProgressStageRepository.ModelToEntity(progressStage, existingStage);
                    updatedProgressStages.Add(existingStage);
                }
            }
            entity.ProgressStages = _unitOfWork.Repositories.WorkOrderProgressStageRepository.ReplaceCollection(existingProgressStages, updatedProgressStages);

            return entity;
        }
    }
}
