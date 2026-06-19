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
        public WorkshopModel Workshop { get; }
        public CreatorModel Creator { get; }
    }
}