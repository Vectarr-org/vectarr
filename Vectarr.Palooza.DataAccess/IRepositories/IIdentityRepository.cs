using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.Model;
using Vectarr.Palooza.DataAccess.IInfrastructure;

namespace Vectarr.Palooza.DataAccess.IRepositories
{
    public interface IIdentityRepository : IEntityRepository<Identity, IdentityModel>
    {
        IdentityModel GetCurrentUserIdentity();
    }
}
