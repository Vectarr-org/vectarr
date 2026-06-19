using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.Model;
using Vectarr.Palooza.Exceptions;
using Vectarr.Palooza.CoreClasses.Enums;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Vectarr.Palooza.Classes.ExternalLogins;
using Microsoft.AspNetCore.WebUtilities;
using Vectarr.Palooza.CoreClasses.Wrappers;
using Vectarr.Palooza.ApiBusiness.IServices;
using Vectarr.Palooza.ApiBusiness.Emailing.EmailTemplates.TemplateClasses;
using Vectarr.Palooza.ApiBusiness.IServices.IUserServices;
using Vectarr.Palooza.ApiBusiness.Emailing.BaseEmailTemplating;
using Vectarr.Palooza.Digifabster;
using Vectarr.Palooza.Digifabster.Classes;

namespace Vectarr.Palooza.ApiBusiness.Services.UserServices
{
    public class UserAuthenticationServices
    {
        private readonly UserManager<UserAccount> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly SignInManager<UserAccount> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly IDigifabsterService _digifabsterService;

        public UserAuthenticationServices(UserManager<UserAccount> userManager,
            SignInManager<UserAccount> signInManager, IJwtTokenService jwtTokenService,
            IUnitOfWork unitOfWork, IEmailSender emailSender, IConfiguration configuration, IDigifabsterService digifabsterService)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            _configuration = configuration;
            _digifabsterService = digifabsterService;
        }

        public string GenerateReferralCode(string email)
        {
            // Get the part of the email before the '@' symbol
            string emailPart = email.Split('@')[0];

            // Remove any dots from the email part
            string referralCode = emailPart.Replace(".", "").ToUpper();

            return referralCode.ToLower();
        }

        public async Task<AuthenticationResult> Register(RegistrationModel user, string referralCode = "")
        {
            ValidateRegistrationModel(user);

            var existingUser = await _userManager.FindByEmailAsync(user.EmailAddress);

            if (existingUser != null)
            {
                throw new ErrorValidationException("An account with a matching email already exists!");
            }

            // Verify Digifabster FIRST for creators before creating the local account
            var digifabsterResult = new DigifabsterResult<RegisterClientResponse>();
            if (user.IsCreator)
            {
                digifabsterResult = await _digifabsterService.RegisterClientAsync(new RegisterClientRequest
                {
                    Email = user.EmailAddress,
                    Password = user.Password,
                    SilentLoginEnabled = true,
                    SendNotification = false,   
                    Surname = user.LastName ?? "",
                    Name = user.FirstName ?? ""
                });

                if (!digifabsterResult.IsSuccess)
                {
                    throw new ErrorValidationException(digifabsterResult.ErrorMessage);
                }
            }

            var referral = _unitOfWork.Repositories.UserAccountRepository.GetByReferralCode(referralCode);
            var newUser = new UserAccount()
            {
                Email = user.EmailAddress,
                UserName = user.EmailAddress,
                Identity = new Identity()
                {
                    EmailAddress = user.EmailAddress,
                    Address = new Address()
                    {
                        Country = CountryEnum.UnitedStates
                    },
                    FirstName = user.FirstName,
                    LastName = user.LastName
                },
                Referral = referral,
                ReferralCode = GenerateReferralCode(user.EmailAddress),
                Application = new Application()
                {

                },
                DigifabsterId = user.IsCreator && digifabsterResult?.Data?.Id != null ? digifabsterResult.Data.Id.Value.ToString() : null
            };

            if (user.IsCreator)
            {
                newUser.Creator = new Creator()
                {
                    Company = user.Creator.Company,
                };
            }
            else if (user.IsWorkshop)
            {
                newUser.Workshop = new Workshop()
                {
                    Name = user.Workshop.Name,
                    CompanyName = user.Workshop.CompanyName,
                    Email = user.EmailAddress,
                    Address = new Address()
                    {
                        Country = user.Workshop.Address.Country,
                        City = user.Workshop.Address.City,
                        State = user.Workshop.Address.State,
                        Street1 = user.Workshop.Address.Street1,
                        Street2 = user.Workshop.Address.Street2,
                        PostalCode = user.Workshop.Address.PostalCode
                    },
                    PhoneNumber = user.Workshop.PhoneNumber,
                };
            }

            var createdUserResult = !user.IsExternal ?
                await _userManager.CreateAsync(newUser, user.Password) :
                 await _userManager.CreateAsync(newUser);

            if (createdUserResult.Succeeded)
            {
                var rolesWrapper = new RolesEnumWrapper();
                var roleResult = await _userManager.AddToRoleAsync(newUser, user.IsWorkshop ? rolesWrapper.GetDisplayText(RolesEnum.Workshop) : rolesWrapper.GetDisplayText(RolesEnum.Creator));
                if (!roleResult.Succeeded)
                {
                    throw new ErrorValidationException($"Registration failed, please contact support.");
                }

                var jwtToken = _jwtTokenService.GenerateJwtToken(newUser);
                if (!user.IsExternal)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                    var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                    EmailTemplateSender sender = new EmailTemplateSender(_emailSender);
                    var userModel = new IdentityModel() { FirstName = newUser.Identity.FirstName, LastName = newUser.Identity.LastName, EmailAddress = newUser.Email };
                    EmailVerification verificationTemplate = new EmailVerification(userModel, "https://vectarr.com/verification/" + code + "/" + newUser.Id);
                    await sender.SendAsync(verificationTemplate, userModel, overrideSubject: "Verification - Vectarr");
                }
                else
                {
                    _unitOfWork.SetEmail(newUser.UserName);
                }

                return new AuthenticationResult()
                {
                    Success = true,
                    Token = jwtToken
                };
            }
            else
            {
                throw new ErrorValidationException(string.Join(Environment.NewLine, createdUserResult.Errors.Select(x => x.Description).ToList()));
            }
        }

        public string GenerateUnsubscribeToken(string userId, string email)
        {
            // Create JWT token with user info
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtConfig:UnsubscribeKey"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim("userId", userId),
                new Claim("userEmail", email)
            }),
                Expires = DateTime.UtcNow.AddYears(1), // Long expiration for unsubscribe links
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        public void ValidateRegistrationModel(RegistrationModel user)
        {
            List<string> errors = new List<string>();
            if (string.IsNullOrEmpty(user.EmailAddress))
            {
                errors.Add("Email is required.");
            }
            //else if (!IsValid(user.EmailAddress))
            //{
            //    throw new ErrorValidationException("Invalid email address.");
            //}

            if (string.IsNullOrEmpty(user.Password) && !user.IsExternal)
            {
                errors.Add("Password is required.");
            }

            if (errors.Any())
            {
                throw new ErrorValidationException(string.Join(Environment.NewLine, errors));
            }
        }

        public async Task<AuthenticationResult> VerifyUserEmailAddress(string token, int userId)
        {
            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var user = _unitOfWork.Repositories.UserAccountRepository.GetEntityById(userId, skipApplicationCheck: true);
            var confirmationResults = await _userManager.ConfirmEmailAsync(user, decodedToken);
            if (!confirmationResults.Succeeded)
            {
                throw new ErrorValidationException("Unable to verify email address, please contact support!");
            }
            else
            {
                _unitOfWork.SetEmail(user.Email);
                _unitOfWork.CurrentUser.EmailConfirmed = true;
                _unitOfWork.GetGenericRepository<UserAccount>().Update(_unitOfWork.CurrentUser);

                var jwtResult = _jwtTokenService.GenerateJwtToken(user);

                return new AuthenticationResult()
                {
                    Success = true,
                    Token = jwtResult
                };
            }
        }

        public AuthenticationResult AuthenticateUser(string email)
        {
            var unitOfWork = new UnitOfWork();
            unitOfWork.SetEmail(email);
            var jwtResult = _jwtTokenService.GenerateJwtToken(unitOfWork.CurrentUser);
            return new AuthenticationResult()
            {
                Success = true,
                Token = jwtResult
            };
        }


        public async Task<AuthenticationResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ErrorValidationException("Email is required.");
            }
            else if (!IsValid(email))
            {
                throw new ErrorValidationException("Invalid email address.");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ErrorValidationException("Password is required.");
            }

            var result = await _signInManager.PasswordSignInAsync(email, password, false, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(email);
                var jwtResult = _jwtTokenService.GenerateJwtToken(user);

                return new AuthenticationResult()
                {
                    Success = true,
                    Token = jwtResult
                };
            }


            throw new ErrorValidationException("Invalid login attempt!");
        }


        public async void Logout()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<AuthenticationResult> ForgotPassword(string email)
        {
            var existingUser = _unitOfWork.Repositories.UserAccountRepository.FindUserByEmail(email);
            if (existingUser == null)
            {
                throw new ErrorValidationException("Invalid email address!");
            }

            var user = await _userManager.FindByEmailAsync(email);
            existingUser.PasswordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            _unitOfWork.SetEmail(existingUser.EmailAddress);
            _unitOfWork.Repositories.UserAccountRepository.Update(existingUser);

            var userIndentity = new IdentityModel()
            {
                EmailAddress = existingUser.EmailAddress
            };
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var link = GenerateResetLink("https://Vectarr.com/", email, token);
            ForgotPassword forgotPasswordEmailTemplate = new ForgotPassword(userIndentity, link);
            forgotPasswordEmailTemplate.Subject = "Reset Password Link - Vectarr";
            EmailTemplateSender sender = new EmailTemplateSender(_emailSender);
            await sender.SendAsync(forgotPasswordEmailTemplate, userIndentity);

            return new AuthenticationResult()
            {
                Success = true
            };
        }

        private string GenerateResetLink(string baseSiteUrl, string email, string code)
        {
            var resetPasswordLink = baseSiteUrl + "reset-password";
            var url = resetPasswordLink + "?email=" + Uri.EscapeDataString(email) + "&token=" + Uri.EscapeDataString(code);
            return url;
        }


        private bool IsValid(string email)
        {
            string regex = @"^[^@\s]+@[^@\s]+\.(com|net|org|gov)$";

            return Regex.IsMatch(email, regex, RegexOptions.IgnoreCase);
        }

        public async Task<AuthenticationResult> ResetPassword(string email, string code, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var result = await _userManager.ResetPasswordAsync(user, code, newPassword);

                if (result.Succeeded)
                {
                    return new AuthenticationResult()
                    {
                        Success = true
                    };
                }

                throw new ErrorValidationException(string.Join(Environment.NewLine, result.Errors.Select(w => w.Description)));
            }


            throw new ErrorValidationException("Rest password failed. Try again or contact support!");
        }

        public AuthenticationInfo GetAuthenticationInfo()
        {
            if (_unitOfWork.CurrentUser != null)
            {
                var user = _unitOfWork.Repositories.IdentityRepository.GetCurrentUserIdentity();
                user.referralCode = _unitOfWork.CurrentUser.ReferralCode;

                return new AuthenticationInfo()
                {
                    Identity = user,
                    Subscription = _unitOfWork.Subscription,
                    Role = _unitOfWork.Role,
                    Workshop = _unitOfWork.Role == RolesEnum.Workshop ? _unitOfWork.Workshop : null,
                    Creator = _unitOfWork.Role == RolesEnum.Creator ? _unitOfWork.Creator : null,
                    IsAdmin = true
                };
            }

            return new AuthenticationInfo();
        }

        public void Unsubscribe(string token)
        {
            try
            {
                // Validate the token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["JwtConfig:UnsubscribeKey"]);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);

                var userId = principal.FindFirst("userId")?.Value;
                var email = principal.FindFirst("userEmail")?.Value;

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email))
                {
                    throw new ErrorValidationException("Could not unsubscribe please contact support.");
                }

                // Update user preferences in the database
                var user = _unitOfWork.Repositories.IdentityRepository.GetById(int.Parse(userId));
                if (user == null || user.EmailAddress != email)
                {
                    throw new ErrorValidationException("Could not unsubscribe please contact support.");
                }

                user.IsEmailSubscribed = false;
                _unitOfWork.Repositories.IdentityRepository.Update(user);
            }
            catch (Exception ex)
            {
                throw new ErrorValidationException("Could not unsubscribe please contact support.");
            }
        }

        //public async Task<AuthenticationResult> ExternalLogin(ExternalPayload payload, string externalCredetial, string referralCode = null)
        //{
        //    //var result = await _signInManager.PasswordSignInAsync(username, password, false, false);
        //    var user = await _userManager.FindByNameAsync(payload.Email);
        //    if (user != null)
        //    {
        //        var jwtResult = _jwtTokenService.GenerateJwtToken(user);

        //        try
        //        {
        //            await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", "Google", externalCredetial));
        //        }
        //        catch (Exception ex)
        //        {

        //        }


        //        return new AuthenticationResult()
        //        {
        //            Success = true,
        //            Token = jwtResult
        //        };
        //    }
        //    else
        //    {
        //        return await Register(new RegistrationModel() { EmailAddress = payload.Email, IsExternal = true }, referralCode);
        //    }
        //}
    }
}
