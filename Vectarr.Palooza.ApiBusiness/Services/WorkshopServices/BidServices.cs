using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.CoreClasses;
using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.Exceptions;

namespace Vectarr.Palooza.ApiBusiness.Services.WorkshopServices
{
    public class BidServices : BaseManager<BidModel>
    {
        private readonly IUnitOfWork _unitOfWork;
        public BidServices(IUnitOfWork unitOfWork) : base(unitOfWork, unitOfWork.Repositories.BidRepository)
        {
            _unitOfWork = unitOfWork;
        }

        protected override BidModel InsertInternal(BidModel item)
        {
            PrepareAndAuthorize(item);
            return base.InsertInternal(item);
        }

        protected override BidModel UpdateInternal(BidModel item)
        {
            PrepareAndAuthorize(item);
            return base.UpdateInternal(item);
        }

        protected override BidModel UpdateIncludeInternal(BidModel item)
        {
            PrepareAndAuthorize(item);
            return base.UpdateIncludeInternal(item);
        }

        /// <summary>
        /// Defaults ship-from to the shop country and rejects bids from non-US shops on US-only RFQs.
        /// ITAR and export-controlled jobs count as US-only even if the stored region was not updated.
        /// </summary>
        private void PrepareAndAuthorize(BidModel item)
        {
            if (item.Job == null || item.Job.Id == 0)
            {
                throw new ErrorValidationException("A bid must target a job.");
            }

            var workshopId = item.Workshop?.Id ?? 0;
            if (_unitOfWork.Workshop != null)
            {
                // A signed-in shop bids as itself. The client cannot point the bid at another shop to skip the region check.
                item.Workshop ??= new WorkshopModel();
                item.Workshop.Id = _unitOfWork.Workshop.Id;
                workshopId = item.Workshop.Id;
            }

            if (workshopId == 0)
            {
                throw new ErrorValidationException("A bid must come from a shop.");
            }

            var workshop = _unitOfWork.Workshop != null && _unitOfWork.Workshop.Id == workshopId
                ? _unitOfWork.Workshop
                : _unitOfWork.Repositories.WorkshopRepository.GetById(workshopId, skipApplicationCheck: true);

            if (workshop == null)
            {
                throw new ErrorValidationException("Shop was not found.");
            }

            var shopCountry = SourcingRegionPolicy.ResolveCountry(workshop.Country, workshop.CountryCodeInput)
                ?? workshop.Address?.Country;
            var requestedShipFrom = SourcingRegionPolicy.ResolveCountry(item.ShipFromCountry, item.ShipFromCountryCodeInput);
            item.ShipFromCountry = requestedShipFrom ?? shopCountry;
            item.DutyEstimate = BidPricing.ResolveDutyEstimate(item.DutyEstimate, item.Tarrifs);

            var job = _unitOfWork.Repositories.JobRepository.GetById(item.Job.Id, skipApplicationCheck: true);
            if (job == null)
            {
                throw new ErrorValidationException("Job was not found.");
            }

            if (!SourcingRegionPolicy.CanShopAccess(shopCountry, job.SourcingRegion, job.IsItar, job.IsExportControlled))
            {
                throw new ErrorValidationException("Non-US shops cannot bid on RFQs that are limited to US shops.");
            }
        }

        public WorkOrderModel AcceptBid(int bidId)
        {
            var bid = _unitOfWork.Repositories.BidRepository.GetById(bidId);

            var workOrder = new WorkOrderModel()
            {
                JobId = bid.Job.Id,
                WorkshopId = bid.Workshop.Id,
                AcceptedBidId = bid.Id,
                Status = WorkOrderStatusEnum.CreatorAccepted,
                CreatorId = _unitOfWork.Creator.Id,
            };

            workOrder = _unitOfWork.Repositories.WorkOrderRepository.Insert(workOrder);

            bid.Status = BidStatusEnum.Accepted;
            Update(bid);

            var job = _unitOfWork.Repositories.JobRepository.GetById(bid.Job.Id);
            job.Status = JobStatusEnum.InProgress;
            _unitOfWork.Repositories.JobRepository.UpdateWithInclude(job);

            return workOrder;
        }
    }
}
