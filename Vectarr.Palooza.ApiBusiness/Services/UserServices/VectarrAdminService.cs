using Microsoft.Extensions.Configuration;
using Vectarr.Palooza.ApiBusiness.IServices.IUserServices;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.Model;

namespace Vectarr.Palooza.ApiBusiness.Services.UserServices
{
    public class VectarrAdminService : IVectarrAdminService
    {
        public const string AdminEmailsConfigKey = "VectarrAdmin:Emails";

        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public VectarrAdminService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public bool IsCurrentUserVectarrAdmin()
        {
            if (_unitOfWork.CurrentUser == null)
            {
                return false;
            }

            return IsVectarrAdmin(_unitOfWork.Identity, _unitOfWork.CurrentUser.Email);
        }

        public bool IsVectarrAdmin(Identity? identity, string? accountEmail = null)
        {
            if (identity == null && string.IsNullOrWhiteSpace(accountEmail))
            {
                return false;
            }

            if (identity?.IsVectarrAdmin == true)
            {
                return true;
            }

            var adminEmails = _configuration.GetSection(AdminEmailsConfigKey).Get<string[]>() ?? Array.Empty<string>();
            if (adminEmails.Length == 0)
            {
                return false;
            }

            return IsListed(adminEmails, identity?.EmailAddress) || IsListed(adminEmails, accountEmail);
        }

        private static bool IsListed(string[] adminEmails, string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            var normalized = email.Trim();
            return adminEmails.Any(e => !string.IsNullOrWhiteSpace(e) && string.Equals(e.Trim(), normalized, StringComparison.OrdinalIgnoreCase));
        }
    }
}
