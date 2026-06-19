using Vectarr.Palooza.ApiBusiness.IServices;
using Vectarr.Palooza.ApiBusiness.Services.JobServices;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Vectarr.Palooza.Model;
using Vectarr.Palooza.ApiBusiness.IServices.IUserServices;

namespace Vectarr.Palooza.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class JobController : BaseController<JobModel>
    {
        private readonly JobServices _jobServices;
        public JobController(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, UserManager<UserAccount> userManager,
            SignInManager<UserAccount> signInManager, IJwtTokenService jwtTokenService, IEmailSender emailSender) : base(unitOfWork, httpContextAccessor)
        {
            _jobServices = new JobServices(unitOfWork, userManager, signInManager, jwtTokenService, emailSender);
        }

        protected override IBaseManager<JobModel> GetManager()
        {
            return _jobServices;
        }

        [HttpPost]
        [Route("GenerateJobQuote")]
        [AllowAnonymous]
        public async Task<AuthenticationResult> GenerateJobQuote(GenerateQuoteAuthModel model)
        {
            return await _jobServices.GenerateJobQuote(model);
        }

        [HttpPost]
        [Route("Publish")]
        public void Publish(int jobId)
        {
            _jobServices.PublishJob(jobId);
        }

        [HttpPost]
        [Route("UnPublish")]
        public void UnPublish(int jobId)
        {
            _jobServices.UnPublishJob(jobId);
        }

        [HttpDelete]
        [Route("Delete")]
        public void Delete(int id)
        {
            _jobServices.Delete(id);
        }
    }
}
