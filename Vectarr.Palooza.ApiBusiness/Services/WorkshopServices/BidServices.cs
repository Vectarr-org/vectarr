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
