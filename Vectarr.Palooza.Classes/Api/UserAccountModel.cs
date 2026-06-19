using Vectarr.Palooza.Classes.BaseApi;

namespace Vectarr.Palooza.Classes.Api
{
    public class UserAccountModel : BaseApiModel
    {
        public string PasswordResetToken { get; set; }
        public int PasswordResetCode { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public int ApplicationId { get; set; }
        public int IdentityId { get; set; }
        public int? ReferralId { get; set; }
        public string? ReferralCode { get; set; }
        public int? CreatorId { get; set; }
        public int? WorkshopId { get; set; }
        public string? DigifabsterId { get; set; }
    }
}
