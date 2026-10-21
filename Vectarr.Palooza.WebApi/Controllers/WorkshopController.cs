using Vectarr.Palooza.ApiBusiness.IServices;
using Vectarr.Palooza.ApiBusiness.Services.WorkshopServices;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.Classes.BaseApi;
using Vectarr.Palooza.CoreClasses.Enums;
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
        /// Admin only. Paged certificate review queue.
        /// GET api/Workshop/CertificationsForReview?reviewStatus=0&amp;pageNumber=1&amp;countPerPage=10&amp;orderBy=&amp;sort=
        /// reviewStatus defaults to Pending (0). sort starting with "d" orders descending, same as Get.
        /// </summary>
        [HttpGet("CertificationsForReview")]
        public ListViewModel<CertificationReviewItemModel> CertificationsForReview(
            CertificationReviewStatusEnum? reviewStatus = null,
            int pageNumber = 1,
            int countPerPage = 10,
            string? orderBy = null,
            string? sort = null)
        {
            return _workshopServices.ListCertificationsForReview(
                reviewStatus,
                pageNumber,
                countPerPage,
                orderBy,
                sort != null && sort.StartsWith("d"));
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
