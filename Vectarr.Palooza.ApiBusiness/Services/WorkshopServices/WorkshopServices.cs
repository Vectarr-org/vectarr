using Vectarr.Palooza.ApiBusiness.Emailing.BaseEmailTemplating;
using Vectarr.Palooza.ApiBusiness.Emailing.EmailTemplates.TemplateClasses;
using Vectarr.Palooza.ApiBusiness.IServices;
using Vectarr.Palooza.ApiBusiness.Services.WorkOrderServices;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.Exceptions;
using Vectarr.Palooza.Model;

namespace Vectarr.Palooza.ApiBusiness.Services.WorkshopServices
{
    public class WorkshopServices : BaseManager<WorkshopModel>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        public WorkshopServices(IUnitOfWork unitOfWork, IEmailSender emailSender) : base(unitOfWork, unitOfWork.Repositories.WorkshopRepository)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
        }

        public List<JobModel> GetAvailableJobs()
        {
            return _unitOfWork.Repositories.JobRepository.GetWorkshopAvailableJobs();
        }

        public WorkOrderModel ConfirmAndGetStarted(int workOrderId, string initialNotes)
        {
            var workOrder = _unitOfWork.Repositories.WorkOrderRepository.GetById(workOrderId);
            workOrder.Status = WorkOrderStatusEnum.Active;
            workOrder.StartedDate = DateTime.UtcNow;

            workOrder.ProgressStages = new List<WorkOrderProgressStageModel>()
            {
                new WorkOrderProgressStageModel()
                {
                     Stage = ProgressStageEnum.JobStarted,
                     Notes = initialNotes
                }
            };


            return _unitOfWork.Repositories.WorkOrderRepository.UpdateWithInclude(workOrder);
        }
        public WorkOrderModel MoveToInProgress(int workOrderId, int predefinedProgressStepId, string note = "")
        {
            var workOrder = _unitOfWork.Repositories.WorkOrderRepository.GetById(workOrderId);
            var jobStartedStage = workOrder.ProgressStages?.FirstOrDefault(s => s.Stage == ProgressStageEnum.JobStarted);

            if (workOrder.Status == WorkOrderStatusEnum.Active &&
                jobStartedStage != null &&
                jobStartedStage.CompletedAt == null)
            {
                jobStartedStage.CompletedAt = DateTime.UtcNow;

                workOrder.ProgressStages.Add(new WorkOrderProgressStageModel()
                {
                    Stage = ProgressStageEnum.InProgress,
                    ProgressUpdates = new List<WorkOrderProgressUpdateModel>()
            {
                new WorkOrderProgressUpdateModel()
                {
                    Notes = note,
                    WorkshopCustomProgressMessageId = predefinedProgressStepId
                }
            }
                });
                return _unitOfWork.Repositories.WorkOrderRepository.UpdateWithInclude(workOrder);
            }
            throw new ErrorValidationException("Failed to move status to in progress.");
        }

        public WorkOrderModel MoveToInQualityCheck(int workOrderId)
        {
            var workOrder = _unitOfWork.Repositories.WorkOrderRepository.GetById(workOrderId);
            var inProgressStage = workOrder.ProgressStages?.FirstOrDefault(s => s.Stage == ProgressStageEnum.InProgress);

            if (workOrder.Status == WorkOrderStatusEnum.Active &&
                inProgressStage != null &&
                inProgressStage.CompletedAt == null)
            {
                 inProgressStage.CompletedAt = DateTime.UtcNow;

                workOrder.ProgressStages.Add(new WorkOrderProgressStageModel()
                {
                    Stage = ProgressStageEnum.QualityCheck,
                });


                var job = _unitOfWork.Repositories.JobRepository.GetById(workOrder.JobId, skipApplicationCheck: true);
                if(job.QualityChecks != null && job.QualityChecks.Count > 0)
                {
                    workOrder.QualityCheckRequests = new List<WorkOrderQualityCheckRequestModel>();
                    foreach (var qualityCheck in job.QualityChecks)
                    {
                        var qcRequest = new WorkOrderQualityCheckRequestModel()
                        {
                            WorkOrderId = workOrder.Id,
                            CheckType = qualityCheck.CheckType,
                            Instructions = qualityCheck.Instructions,
                        };
                        _unitOfWork.Repositories.WorkOrderQualityCheckRequestRepository.Insert(qcRequest);
                    }
                }
              

                return _unitOfWork.Repositories.WorkOrderRepository.UpdateWithInclude(workOrder);
            }
            throw new ErrorValidationException("Failed to move status to quality check.");
        }

        public WorkOrderQualityCheckRequestModel SubmitQualityCheckRequest(WorkOrderQualityCheckRequestModel request)
        {
            var qcRequest = _unitOfWork.Repositories.WorkOrderQualityCheckRequestRepository.GetById(request.Id);
            qcRequest.CompletedAt = DateTime.UtcNow;
            qcRequest.Attachments = request.Attachments;

            return _unitOfWork.Repositories.WorkOrderQualityCheckRequestRepository.UpdateWithInclude(qcRequest);
        }

        public WorkOrderModel PassQCAndShip(int workOrderId)
        {
            var workOrderServices = new WorkOrderServices.WorkOrderServices(_unitOfWork);
            var workOrder = workOrderServices.GetItem(workOrderId);

            if (workOrder.QualityCheckStatus != null && !workOrder.QualityCheckStatus.CanProceedToShipping)
                throw new ErrorValidationException("Cannot proceed yet");

            var qcStage = workOrder.ProgressStages?.FirstOrDefault(s => s.Stage == ProgressStageEnum.QualityCheck);

            if (qcStage == null || qcStage.CompletedAt != null)
                throw new ErrorValidationException("Invalid quality check stage state");

            qcStage.CompletedAt = DateTime.UtcNow;

            workOrder.ProgressStages.Add(new WorkOrderProgressStageModel()
            {
                Stage = ProgressStageEnum.Shipping,
            });

            return _unitOfWork.Repositories.WorkOrderRepository.UpdateWithInclude(workOrder);
        }

        public WorkOrderModel AddShippingInformation(WorkOrderProgressStageModel progressStage)
        {
            var workOrder = _unitOfWork.Repositories.WorkOrderRepository.GetById(progressStage.WorkOrderId);

            var shippingStage = workOrder.ProgressStages?.FirstOrDefault(s => s.Stage == ProgressStageEnum.Shipping);

            if (shippingStage == null || shippingStage.CompletedAt != null)
                throw new ErrorValidationException("Work order is not at the shipping stage");

            shippingStage.TrackingNumber = progressStage.TrackingNumber;
            shippingStage.Carrier = progressStage.Carrier;
            shippingStage.EstimatedDelivery = progressStage.EstimatedDelivery;
            shippingStage.Notes = progressStage.Notes;
            shippingStage.CompletedAt = DateTime.UtcNow;

            workOrder.ProgressStages.Add(new WorkOrderProgressStageModel()
            {
                Stage = ProgressStageEnum.Received,
            });

            return _unitOfWork.Repositories.WorkOrderRepository.UpdateWithInclude(workOrder);
        }

        public async Task SendPartnerSubmissionEmail(BecomeAPartnerRequest request)
        {
            EmailTemplateSender sender = new EmailTemplateSender(_emailSender);
            PartnerRequest partnerrequestEmail = new PartnerRequest(request);
            await sender.SendAsync(partnerrequestEmail, new IdentityModel() { EmailAddress = "info@vectarr.com" }, overrideSubject: "Partner Request - Vectarr");
        }
    }
}
