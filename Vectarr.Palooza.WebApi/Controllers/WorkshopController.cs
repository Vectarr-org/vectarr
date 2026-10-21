using Vectarr.Palooza.ApiBusiness.IServices;
using Vectarr.Palooza.ApiBusiness.Services.WorkshopServices;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Vectarr.Palooza.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class WorkshopController : BaseController<WorkshopModel>
    {
        private readonly WorkshopServices _workshopServices;
        public WorkshopController(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IEmailSender emailSender) : base(unitOfWork, httpContextAccessor)
        {
            _workshopServices = new WorkshopServices(unitOfWork, emailSender);
        }

        protected override IBaseManager<WorkshopModel> GetManager()
        {
            return _workshopServices;
        }

        [HttpGet("GetAvailableJobs")]
        public List<JobModel> GetAvailableJobs()
        {
            return _workshopServices.GetAvailableJobs();
        }

        /// <summary>
        /// Admin only. Sets certificate review status, reviewer, and reviewed-at.
        /// Body: certificationId, reviewStatus (1 verified, 2 rejected), rejectionReason, expiryDate.
        /// </summary>
        [HttpPost("ReviewCertification")]
        public WorkshopCertificationModel ReviewCertification(CertificationReviewRequest request)
        {
            return _workshopServices.ReviewCertification(request);
        }

        [HttpPost("ConfirmAndGetStarted")]
        public WorkOrderModel ConfirmAndGetStarted(int workOrderId, string initialNotes = "")
        {
            return _workshopServices.ConfirmAndGetStarted(workOrderId, initialNotes);
        }

        [HttpPost("MoveToInProgress")]
        public WorkOrderModel MoveToInProgress(int workOrderId, int predefinedProgressStepId, string note = "")
        {
            return _workshopServices.MoveToInProgress(workOrderId, predefinedProgressStepId, note);
        }

        [HttpPost("MoveToInQualityCheck")]
        public WorkOrderModel MoveToInQualityCheck(int workOrderId)
        {
            return _workshopServices.MoveToInQualityCheck(workOrderId);
        }

        [HttpPost("SubmitQualityCheckRequest")]
        public WorkOrderQualityCheckRequestModel SubmitQualityCheckRequest(WorkOrderQualityCheckRequestModel request)
        {
            return _workshopServices.SubmitQualityCheckRequest(request);
        }

        [HttpPost("PassQCAndShip")]
        public WorkOrderModel PassQCAndShip(int workOrderId)
        {
            return _workshopServices.PassQCAndShip(workOrderId);
        }

        [HttpPost("AddShippingInformation")]
        public WorkOrderModel AddShippingInformation(WorkOrderProgressStageModel progressStage)
        {
            return _workshopServices.AddShippingInformation(progressStage);
        }

        [HttpPost("SendPartnerSubmissionEmail")]
        [AllowAnonymous]
        public async void SendPartnerSubmissionEmail(BecomeAPartnerRequest request)
        {
            await _workshopServices.SendPartnerSubmissionEmail(request);
        } 
    }
}
