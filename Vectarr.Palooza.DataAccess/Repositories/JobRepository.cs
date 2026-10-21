using System.Linq.Expressions;
using LinqKit;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.Model;
using Microsoft.EntityFrameworkCore;
using Vectarr.Palooza.DataAccess.IInfrastructure;
using Vectarr.Palooza.DataAccess.Infrastructure;
using Vectarr.Palooza.DataAccess.IRepositories;
using Vectarr.Palooza.CoreClasses;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.DataAccess.Repositories
{
    public class JobRepository : BaseRepository<Job, JobModel>, IJobRepository, IApplicationRepository<Job>
    {
        private readonly IUnitOfWork _unitOfWork;
        public JobRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Expression<Func<Job, bool>> ApplicationPredicate(int applicationId)
        {
            return entity => entity.Creator.UserAccount.Application.Id == applicationId;
        }

        public override Expression<Func<Job, JobModel>> GetEntityToModelExpression()
        {
            var imageExpression = _unitOfWork.Repositories.ImageRepository.GetEntityToModelExpression().Expand();
            var bidExpression = _unitOfWork.Repositories.BidRepository.GetEntityToModelExpression().Expand();
            var creatorExpression = _unitOfWork.Repositories.CreatorRepository.GetEntityToModelExpression().Expand();
            var qualityCheckExpression = _unitOfWork.Repositories.JobQualityCheckRequestRepository.GetEntityToModelExpression().Expand();
            return entity => new JobModel()
            {
                Id = entity.Id,
                ManufacturingType = entity.ManufacturingType,
                SourceFinish = entity.SourceFinish,
                Tolerance = entity.Tolerance,
                DateCreated = entity.CreatedDate,
                CostVsSpeed = entity.CostVsSpeed,
                Material = entity.Material,
                AdditionalNotes = entity.AdditionalNotes,
                Quantity = entity.Quantity,
                PublishDate = entity.PublishDate,
                ManualPricing = entity.ManualPricing,
                InitialPrice = entity.InitialPrice,
                Status = entity.Status,
                Bids = entity.Bids.AsQueryable().Where(s => !s.IsRemoved && !s.IsHidden).Select(bidExpression).ToList(),
                Files = entity.Files.AsQueryable().Where(s => !s.IsRemoved && !s.IsHidden).Select(imageExpression).ToList(),
                QualityChecks = entity.QualityChecks.AsQueryable().Where(s => !s.IsRemoved && !s.IsHidden).Select(qualityCheckExpression).ToList(),
                Creator = creatorExpression.Invoke(entity.Creator),
                Title = entity.Title,
                Description = entity.Description,
                WorkOrderId = entity.WorkOrder != null ? entity.WorkOrder.Id : null,
                RequireQcConfirmation = entity.RequireQcConfirmation,
                QCPercentage = entity.QCPrecentage,
                AutoAcceptQcReport = entity.AutoAcceptQcReport,
                SourcingRegion = entity.SourcingRegion,
                IsItar = entity.IsItar,
                IsExportControlled = entity.IsExportControlled
            };
        }

        public override IQueryable<Job> GetIncludes(IQueryable<Job> query)
        {
            return query.Include(s => s.Files).Include(s => s.QualityChecks);
        }

        protected override Job ModelToEntityInternal(JobModel model, Job entity)
        {
            entity.Tolerance = model.Tolerance;
            entity.ManufacturingType = model.ManufacturingType;
            entity.SourceFinish = model.SourceFinish;
            entity.AdditionalNotes = model.AdditionalNotes;
            entity.CostVsSpeed = model.CostVsSpeed;
            entity.Title = model.Title;
            entity.Description = model.Description;
            entity.PublishDate = model.PublishDate;
            entity.Material = model.Material;
            entity.Quantity = model.Quantity;
            entity.ManualPricing = model.ManualPricing;
            entity.InitialPrice = model.InitialPrice;
            entity.Status = model.Status;
            entity.AutoAcceptQcReport = model.AutoAcceptQcReport;

            entity.RequireQcConfirmation = model.RequireQcConfirmation;
            entity.QCPrecentage = model.QCPercentage;
            entity.IsItar = model.IsItar;
            entity.IsExportControlled = model.IsExportControlled;
            entity.SourcingRegion = SourcingRegionPolicy.Resolve(model.SourcingRegion, model.IsItar, model.IsExportControlled);

            if (model.Id == 0)
            {
                entity.CreatorId = _unitOfWork.Creator.Id;
            }

            var existingImages = entity.Files ?? new List<Image>();
            var updatedImages = new List<Image>();
            if (model.Files != null)
            {
                foreach (var image in model.Files.ToList())
                {
                    var existingImage = existingImages.FirstOrDefault(x => x.Id == image.Id) ?? _unitOfWork.Repositories.ImageRepository.GetEntityById(image.Id);
                    existingImage.JobId = entity.Id;
                    updatedImages.Add(existingImage);
                }
            }
            entity.Files = _unitOfWork.Repositories.ImageRepository.ReplaceCollection(existingImages, updatedImages);

            var existingJobQualityChecks = entity.QualityChecks ?? new List<JobQualityCheckRequest>();
            var updatedJobQualityChecks = new List<JobQualityCheckRequest>();
            if (model.QualityChecks != null)
            {
                foreach (var qualityCheck in model.QualityChecks.ToList())
                {
                    var existingQualityCheck = existingJobQualityChecks.FirstOrDefault(x => x.Id == qualityCheck.Id) ?? new JobQualityCheckRequest();
                    existingQualityCheck = _unitOfWork.Repositories.JobQualityCheckRequestRepository.ModelToEntity(qualityCheck, existingQualityCheck);
                    updatedJobQualityChecks.Add(existingQualityCheck);
                }
            }
            entity.QualityChecks = _unitOfWork.Repositories.JobQualityCheckRequestRepository.ReplaceCollection(existingJobQualityChecks, updatedJobQualityChecks);


            return entity;
        }

        protected override void RegisterDependencies()
        {
            RegisterDependentProperty(x => x.Bids, UnitOfWork.Repositories.BidRepository);
        }

        public List<JobModel> GetWorkshopAvailableJobs()
        {
            var workshop = _unitOfWork.Workshop;

            var capabilityValues = workshop.WorkshopCapabilities?
                .Select(s => s.Capability)
                .ToList();

            var shopCountry = workshop.Country ?? workshop.Address?.Country;
            // Same boolean as SourcingRegionPolicy.VisibleInShopJobQuery. Non-US shops, including shops
            // with no country, do not receive US-only, ITAR, or export-controlled RFQs.
            var shopIsInUnitedStates = shopCountry == CountryEnum.UnitedStates;

            var jobs = GetEntitiesQueryable(skipApplicationCheck: true)
                .Where(w => w.Status == JobStatusEnum.Published
                && !w.IsRemoved && !w.ManualPricing
                && capabilityValues != null
                && capabilityValues.Contains(w.ManufacturingType) && w.Bids.Count == 0
                && (shopIsInUnitedStates || (w.SourcingRegion != SourcingRegionEnum.UsShopsOnly && !w.IsItar && !w.IsExportControlled)))
                .OrderByDescending(w => w.CreatedDate)
                .Select(GetEntityToModelExpression())
                .ToList();

            return SourcingRegionPolicy.FilterVisible(
                jobs,
                shopCountry,
                job => job.SourcingRegion,
                job => job.IsItar,
                job => job.IsExportControlled).ToList();
        }

    }
}
