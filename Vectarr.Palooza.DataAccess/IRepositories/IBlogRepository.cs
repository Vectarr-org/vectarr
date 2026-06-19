using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.Model;
using Vectarr.Palooza.DataAccess.IInfrastructure;

namespace Vectarr.Palooza.DataAccess.IRepositories
{
    public interface IBlogRepository : IEntityRepository<Blog, BlogModel>
    {
    }
}
