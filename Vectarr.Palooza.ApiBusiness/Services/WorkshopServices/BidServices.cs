using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.DataAccess;

namespace Vectarr.Palooza.ApiBusiness.Services.WorkshopServices
{
    public class BidServices : BaseManager<BidModel>
    {
        private readonly IUnitOfWork _unitOfWork;
        public BidServices(IUnitOfWork unitOfWork) : base(unitOfWork, unitOfWork.Repositories.BidRepository)
        {
            _unitOfWork = unitOfWork;
        }

        public WorkOrderModel AcceptBid(int bidId)
        {
            var bid = _unitOfWork.Repositories.BidRepository.GetById(bidId);

            // Shop platform fee is not charged or stored yet. When payout is added, call
            // ShopPlatformFeeCalculator.Calculate on the accepted order price (bid.Amount, or
            // WorkOrder.CurrentPrice once that field is maintained). Tiers are in ShopPlatformFee
            // appsettings. Do not use InstantQuote:InitialFeePercent; that is the buyer's deposit.

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
