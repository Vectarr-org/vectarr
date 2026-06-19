using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.Exceptions;
using Vectarr.Palooza.Model;

namespace Vectarr.Palooza.ApiBusiness.Services.WorkOrderServices
{
    public class WorkOrderServices : BaseManager<WorkOrderModel>
    {
        private readonly IUnitOfWork _unitOfWork;
        public WorkOrderServices(IUnitOfWork unitOfWork) : base(unitOfWork, unitOfWork.Repositories.WorkOrderRepository)
        {
            _unitOfWork = unitOfWork;
        }

        protected override List<WorkOrderModel> LoadAdditionalInfo(List<WorkOrderModel> data)
        {
            foreach (var workOrder in data)
            {
                if (workOrder.ProgressStages != null && workOrder.ProgressStages.Any(s => s.Stage == CoreClasses.Enums.ProgressStageEnum.QualityCheck))
                {
                    var qualityCheckStage = workOrder.ProgressStages.First(s => s.Stage == CoreClasses.Enums.ProgressStageEnum.QualityCheck);
                    workOrder.QualityCheckStatus = GetQualityCheckStatus(qualityCheckStage, workOrder.QualityCheckRequests ?? new List<WorkOrderQualityCheckRequestModel>());
                }
            }

            return data;
        }

        public WorkOrderModel UploadAdditionalFiles(int workOrderId, List<ImageModel> files)
        {
            var workOrder = _unitOfWork.Repositories.WorkOrderRepository.GetById(workOrderId);
            workOrder.AdditionalFiles = files;
            return Update(workOrder);
        }

        private static readonly TimeSpan QualityCheckWindow = TimeSpan.FromDays(3);

        public QualityCheckStatusModel GetQualityCheckStatus(WorkOrderProgressStageModel stage, ICollection<WorkOrderQualityCheckRequestModel> requests)
        {
            var now = DateTime.UtcNow;
            var deadline = stage.DateCreated.Add(QualityCheckWindow);
            var hasRequests = requests.Any();
            var allRequestsCompleted = hasRequests && requests.All(r => r.CompletedAt != null);

            bool canProceed;
            if (hasRequests)
            {
                // Creator requested checks - just need to complete them
                canProceed = allRequestsCompleted;
            }
            else
            {
                // No requests - must wait for 2-day window
                canProceed = stage.WorkshopSubmittedAt.HasValue && now >= deadline;
            }

            return new QualityCheckStatusModel
            {
                StageStartedAt = stage.DateCreated,
                CreatorDeadline = deadline,
                CreatorCanStillRequestChecks = !hasRequests && now < deadline,
                HasRequests = hasRequests,
                PendingRequestCount = requests.Count(r => r.CompletedAt == null),
                AllRequestsCompleted = allRequestsCompleted,
                CanProceedToShipping = canProceed
            };
        }

        public WorkOrderModel SubmitQualityCheckRequests(int workOrderId, List<QualityCheckTypeEnum> requestedChecks)
        {
            var workOrder = GetItem(workOrderId);
            if (workOrder.QualityCheckStatus != null && !workOrder.QualityCheckStatus.CreatorCanStillRequestChecks)
                throw new ErrorValidationException("QC request window has closed");

            foreach (var checkType in requestedChecks)
            {
                _unitOfWork.Repositories.WorkOrderQualityCheckRequestRepository.Insert(new WorkOrderQualityCheckRequestModel
                {
                    WorkOrderId = workOrderId,
                    CheckType = checkType
                });
            }

            return GetItem(workOrderId);
        }

        public WorkOrderModel ConfirmReceipt(WorkOrderProgressStageModel stage)
        {
            var workOrder = GetItem(stage.WorkOrderId);
            var receivedStage = workOrder.ProgressStages?.FirstOrDefault(s => s.Stage == ProgressStageEnum.Received);

            if (receivedStage == null || receivedStage.CompletedAt != null)
                throw new ErrorValidationException("Work order is not at the received stage");

            receivedStage.Notes = stage.Notes;
            receivedStage.CompletedAt = DateTime.UtcNow;
            receivedStage.Condition = stage.Condition;
            receivedStage.StageImages = stage.StageImages;

            workOrder.ProgressStages.Add(new WorkOrderProgressStageModel()
            {
                Stage = ProgressStageEnum.Completed,
                CompletedAt = DateTime.UtcNow
            });

            workOrder.Status = WorkOrderStatusEnum.Completed;
            workOrder.CompletedDate = DateTime.UtcNow;

            return _unitOfWork.Repositories.WorkOrderRepository.UpdateWithInclude(workOrder);
        }

        public WorkOrderQualityCheckRequestModel ApproveQCCheck(WorkOrderQualityCheckRequestModel qc)
        {
            var qualityReport = _unitOfWork.Repositories.WorkOrderQualityCheckRequestRepository.GetById(qc.Id);
            qualityReport.Passed = true;
            qualityReport.CompletedAt = DateTime.UtcNow;
            return _unitOfWork.Repositories.WorkOrderQualityCheckRequestRepository.UpdateWithInclude(qualityReport);
        }

        public WorkOrderQualityCheckRequestModel RejectQCCheck(WorkOrderQualityCheckRejectionModel qcRejection)
        {
            var qualityReport = _unitOfWork.Repositories.WorkOrderQualityCheckRequestRepository.GetById(qcRejection.QualityCheckRequestId);

            // Check rejection count
            var rejectionCount = qualityReport.RejectedCount;
            if (rejectionCount >= 3)
            {
                throw new Exception("Maximum rejection limit (3) reached");
                // Or auto-approve instead
            }

            // Create new rejection record
            var rejection = new WorkOrderQualityCheckRejection
            {
                QualityCheckRequestId = qcRejection.QualityCheckRequestId,
                RejectionReason = qcRejection.RejectionReason,
            };
            _unitOfWork.Repositories.WorkOrderQualityCheckRejectionRepository.Insert(rejection);

            // Reset the report for resubmission
            qualityReport.Passed = null;
            qualityReport.CompletedAt = null;  // Reset so workshop can resubmit

            return _unitOfWork.Repositories.WorkOrderQualityCheckRequestRepository.UpdateWithInclude(qualityReport);
        }
    }
}
