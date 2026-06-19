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
    }
}
