using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Vectarr.Palooza.ApiBusiness.Services.UserServices;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.ApiBusiness.IServices.IUserServices;
using Vectarr.Palooza.Model;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.Exceptions;
using Vectarr.Palooza.ApiBusiness.IServices;
using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.Classes.ExternalLogins;

namespace Vectarr.Palooza.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class UserController : SystemController
    {
        private readonly UserAuthenticationServices _userAuthenticationServices;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        public UserController(UserManager<UserAccount> userManager,
            SignInManager<UserAccount> signInManager,
            IJwtTokenService jwtTokenService,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            IEmailSender emailSender, IConfiguration configuration, UserAuthenticationServices userAuthenticationServices) : base(unitOfWork, httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _emailSender = emailSender;
            _userAuthenticationServices = userAuthenticationServices;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Register")]
        public async Task<AuthenticationResult> Register(RegistrationModel user, string referralCode = "")
        {
            return await _userAuthenticationServices.Register(user, referralCode);
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("Login")]
        public async Task<AuthenticationResult> Login(string username, string password)
        {
            try
            {
                return await _userAuthenticationServices.Login(username, password);
            }
            catch (Exception ex)
            {
                throw new ErrorValidationException(ex.Message);
            }
        }

        [HttpPost]
        [Route("Logout")]
        public async void Logout()
        {
            _userAuthenticationServices.Logout();
            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        [HttpPost]
        [Route("ForgotPassword")]
        [AllowAnonymous]
        public async Task<AuthenticationResult> ForgotPassword(string email)
        {
            return await _userAuthenticationServices.ForgotPassword(email);
        }


        [HttpPost]
        [Route("ResetPassword")]
        [AllowAnonymous]
        public async Task<AuthenticationResult> ResetPassword(string token, string email, string password)
        {
            return await _userAuthenticationServices.ResetPassword(email, token, password);
        }

        [HttpPost]
        [Route("VerifyEmail")]
        [AllowAnonymous]
        public async Task<AuthenticationResult> VerifyEmail(string code, int userId)
        {
            return await _userAuthenticationServices.VerifyUserEmailAddress(code, userId);
        }



        [HttpPost]
        [Route("Unsubscribe")]
        public void Unsubscribe(string token)
        {
            _userAuthenticationServices.Unsubscribe(token);
        }

        //[AllowAnonymous]
        //[HttpPost]
        //[Route("GoogleLogin")]
        //public async Task<AuthenticationResult> GoogleLogin(string credential, string referralCode = "")
        //{
        //    var settings = new GoogleJsonWebSignature.ValidationSettings()
        //    {
        //        Audience = new List<string> { "1047620951128-agcccbj1l45lnathr7j6dkj9osm272sp.apps.googleusercontent.com" }
        //    };

        //    var payload = await GoogleJsonWebSignature.ValidateAsync(credential, settings);

        //    return await _userAuthenticationServices.ExternalLogin(new ExternalPayload()
        //    { Email = payload.Email, Username = payload.Email }, credential, referralCode);
        //}
    }
}
