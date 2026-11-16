using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.Model;
using Vectarr.Palooza.DataAccess.IRepositories;

namespace Vectarr.Palooza.DataAccess
{
    public interface IUnitOfWork
    {
        int ApplicationId { get; }
        void SetEmail(string email);
        UserAccount CurrentUser { get; }
        ApplicationModel Application { get; }
        IGenericRepository<T> GetGenericRepository<T>() where T : class, IBaseModel, new();
        IRepositoriesContainer Repositories { get; }
        Identity Identity { get; }
        ApplicationSubscriptionModel Subscription { get; }
        public RolesEnum Role { get; }
        /// <summary>True after the signed-in user's role was read. The default enum value is not treated as Admin.</summary>
        public bool RoleWasResolved { get; }
        /// <summary>True when the user is listed in VectarrAdmin:Emails. Combined with Identity.IsVectarrAdmin.</summary>
        public bool HasMarketplaceAdminVisibility { get; }
        void SetMarketplaceAdminVisibility(bool isAdmin);
        /// <summary>Set while the signed-in shop's own profile is being loaded, before <see cref="Workshop"/> is assigned.</summary>
        public int? WorkshopIdBeingLoaded { get; }
        public WorkshopModel Workshop { get; }
        public CreatorModel Creator { get; }
    }
}