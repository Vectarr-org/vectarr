using Microsoft.AspNetCore.Mvc;
using Vectarr.Palooza.ApiBusiness.Services.UserServices;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.ApiBusiness.IServices;
using Vectarr.Palooza.Exceptions;
using Microsoft.AspNetCore.Authorization;

namespace Vectarr.Palooza.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class IdentityController : BaseController<IdentityModel>
    {
        private readonly IdentityServices _identityServices;
        private readonly ContactServices _contactServices;

        public IdentityController(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IEmailSender emailSender) : base(unitOfWork, httpContextAccessor)
        {
            _identityServices = new IdentityServices(unitOfWork);
            _contactServices = new ContactServices(emailSender);
        }

        protected override IBaseManager<IdentityModel> GetManager()
        {
            return _identityServices;
        }

        [HttpPost]
        [Route("Contact")]
        [AllowAnonymous]
        public async Task Contact(ContactModel contact)
        {
            await _contactServices.SendContactEmail(contact);
        }
    }
}
