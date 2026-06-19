using Microsoft.AspNetCore.Identity;

namespace Vectarr.Palooza.Model
{
    public class UserAccount : IdentityUser<int>, IBaseModel
    {
        public UserAccount()
        {
            CreatedDate = DateTime.UtcNow;
            LastModified = DateTime.UtcNow;
        }

        public virtual Identity Identity { get; set; }
        public DateTime LastModified { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsRemoved { get; set; }
        public bool IsHidden { get; set; }
        public virtual Application Application { get; set; }
        public int? ReferralId { get; set; }
        public virtual UserAccount? Referral { get; set; }
        public string? ReferralCode { get; set; }

        public virtual Creator? Creator { get; set; }
        public virtual Workshop? Workshop { get; set; }
        public string? DigifabsterId { get; set; }
    }
}
