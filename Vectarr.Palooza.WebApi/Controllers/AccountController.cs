using Microsoft.AspNetCore.Mvc;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.ApiBusiness.Services.UserServices;
using Vectarr.Palooza.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Vectarr.Palooza.CoreClasses.Enums;
using System.IO;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Vectarr.Palooza.ApiBusiness.IServices;
using Vectarr.Palooza.CoreClasses.Wrappers;

namespace Vectarr.Palooza.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : SystemController
    {
        private readonly UserAuthenticationServices _userAuthenticationServices;
        private readonly IImageManager _imageManager;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IUnitOfWork _unitOfWork;
        public AccountController(IUnitOfWork unitOfWork, IImageManager imageManager, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment hostingEnvironment, UserAuthenticationServices userAuthenticationServices) : base(unitOfWork, httpContextAccessor)
        {
            _imageManager = imageManager;
            _userAuthenticationServices = userAuthenticationServices;
            _hostingEnvironment = hostingEnvironment;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("AuthInfo")]
        public AuthenticationInfo AuthInfo()
        {
            return _userAuthenticationServices.GetAuthenticationInfo();
        }

    }
}
