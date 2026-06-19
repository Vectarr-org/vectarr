using Vectarr.Palooza.ApiBusiness.IServices;
using Vectarr.Palooza.ApiBusiness.Services.CreatorServices;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.ApiBusiness.Services.WorkOrderServices;

namespace Vectarr.Palooza.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class CreatorController : BaseController<CreatorModel>
    {
        private readonly CreatorServices _creatorServices;
        private readonly WorkOrderServices _workOrderServices;
        public CreatorController(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, httpContextAccessor)
        {
            _creatorServices = new CreatorServices(unitOfWork);
            _workOrderServices = new WorkOrderServices(unitOfWork);
        }

        protected override IBaseManager<CreatorModel> GetManager()
        {
            return _creatorServices;
        }

        [HttpPost("SubmitQualityCheckRequests")]
        public WorkOrderModel SubmitQualityCheckRequests(int workOrderId, List<QualityCheckTypeEnum> requestedChecks)
        {
            return _workOrderServices.SubmitQualityCheckRequests(workOrderId, requestedChecks);
        }

        [HttpPost("ConfirmReceipt")]
        public WorkOrderModel ConfirmReceipt(WorkOrderProgressStageModel stage)
        {
            return _workOrderServices.ConfirmReceipt(stage);
        }

        [HttpPost("ApproveQCCheck")]
        public WorkOrderQualityCheckRequestModel ApproveQCCheck(WorkOrderQualityCheckRequestModel qc)
        {
            return _workOrderServices.ApproveQCCheck(qc);
        }

        [HttpPost("RejectQCCheck")]
        public WorkOrderQualityCheckRequestModel RejectQCCheck(WorkOrderQualityCheckRejectionModel qcRejection)
        {
            return _workOrderServices.RejectQCCheck(qcRejection);
        }
    }
}
