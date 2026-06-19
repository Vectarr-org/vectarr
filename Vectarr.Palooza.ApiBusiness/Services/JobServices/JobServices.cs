using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.ApiBusiness.Services;
using Microsoft.AspNetCore.Identity;
using Vectarr.Palooza.Model;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Vectarr.Palooza.ApiBusiness.Emailing.BaseEmailTemplating;
using Vectarr.Palooza.ApiBusiness.Emailing.EmailTemplates.TemplateClasses;
using Vectarr.Palooza.ApiBusiness.IServices;
using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.CoreClasses.Wrappers;
using Vectarr.Palooza.Exceptions;
using Vectarr.Palooza.ApiBusiness.IServices.IUserServices;
using Microsoft.AspNetCore.Mvc;
using Vectarr.Palooza.ApiBusiness.Services.UserServices;

namespace Vectarr.Palooza.ApiBusiness.Services.JobServices
{
    public class JobServices : BaseManager<JobModel>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<UserAccount> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly SignInManager<UserAccount> _signInManager;
        public JobServices(IUnitOfWork unitOfWork, UserManager<UserAccount> userManager, SignInManager<UserAccount> signInManager, IJwtTokenService jwtTokenService, IEmailSender emailSender) : base(unitOfWork, unitOfWork.Repositories.JobRepository)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _emailSender = emailSender;
            _signInManager = signInManager;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<AuthenticationResult> GenerateJobQuote(GenerateQuoteAuthModel model)
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);

            // Step 1: Email check only (no password provided)
            if (model.Password == null)
            {
                return new AuthenticationResult
                {
                    EmailAlreadyRegistered = existingUser != null,
                    Success = true
                };
            }

            // Step 2: Password provided - handle existing user login
            if (existingUser != null)
            {
                var emailConfirmed = await _userManager.IsEmailConfirmedAsync(existingUser);

                if (!emailConfirmed)
                {
                    throw new ErrorValidationException("Generate quote failed, please contact support.");
                }
                else
                {
                    var jwtResult = _jwtTokenService.GenerateJwtToken(existingUser);

                    _unitOfWork.SetEmail(existingUser.Email);

                    var job = _unitOfWork.Repositories.JobRepository.Insert(model.Job);

                    return new AuthenticationResult
                    {
                        Success = true,
                        Token = jwtResult,
                        Job = job,
                        EmailAlreadyRegistered = true
                    };
                }
            }
            else
            {
                // Step 3: Password provided - create new user
                var newUser = new UserAccount
                {
                    Email = model.Email,
                    UserName = model.Email,
                    Identity = new Identity { EmailAddress = model.Email },
                    Creator = new Creator(),
                    Application = new Application()
                };

                var createdUserResult = await _userManager.CreateAsync(newUser, model.Password);

                if (!createdUserResult.Succeeded)
                {
                    var errors = string.Join(Environment.NewLine, createdUserResult.Errors.Select(x => x.Description));
                    throw new ErrorValidationException(errors);
                }

                // Add role
                var rolesWrapper = new RolesEnumWrapper();
                var roleResult = await _userManager.AddToRoleAsync(newUser, rolesWrapper.GetDisplayText(RolesEnum.Creator));

                if (!roleResult.Succeeded)
                {
                    throw new ErrorValidationException("Registration failed, please contact support.");
                }

                // Send verification email
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                var emailSender = new EmailTemplateSender(_emailSender);
                var userModel = new IdentityModel
                {
                    FirstName = newUser.Identity.FirstName,
                    LastName = newUser.Identity.LastName,
                    EmailAddress = newUser.Email
                };

                var verificationUrl = $"https://vectarr.com/verification/{code}/{newUser.Id}";
                var verificationTemplate = new EmailVerification(userModel, verificationUrl);
                await emailSender.SendAsync(verificationTemplate, userModel, overrideSubject: "Email Verification - Vectarr");

                _unitOfWork.SetEmail(newUser.Email);
                var job = _unitOfWork.Repositories.JobRepository.Insert(model.Job);

                return new AuthenticationResult
                {
                    Success = true,
                    Job = job
                };
            }
        }

        public void PublishJob(int jobId)
        {
            var job = GetItem(jobId);
            job.Status = JobStatusEnum.Published;
            job.PublishDate = DateTime.UtcNow;
            _unitOfWork.Repositories.JobRepository.UpdateWithInclude(job);
        }

        public void UnPublishJob(int jobId)
        {
            var job = GetItem(jobId);
            if (job.Status == JobStatusEnum.Published && (job.Bids == null || job.Bids.Count == 0))
            job.Status = JobStatusEnum.Draft;
            job.PublishDate = null;
            _unitOfWork.Repositories.JobRepository.UpdateWithInclude(job);
        }
    }
}
