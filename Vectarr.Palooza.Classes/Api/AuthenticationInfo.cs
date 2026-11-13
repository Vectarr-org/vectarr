using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.Classes.Api
{
    public class AuthenticationInfo
    {
        public IdentityModel Identity { get; set; }
        public ApplicationSubscriptionModel? Subscription { get; set; }
        public RolesEnum Role { get; set; }
        public CreatorModel? Creator { get; set; }
        public WorkshopModel? Workshop { get; set; }
        public bool IsAdmin { get; set; }

        /// <summary>
        /// True when the user is a Vectarr admin (Identity.IsVectarrAdmin or listed in VectarrAdmin:Emails).
        /// The frontend should rely on this flag (IsAdmin is currently always true).
        /// </summary>
        public bool IsVectarrAdmin { get; set; }
    }
}
