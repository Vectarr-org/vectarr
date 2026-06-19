using Vectarr.Palooza.Model;

namespace Vectarr.Palooza.DataAccess.IInfrastructure
{
    public interface ISimpleGetEntityRepository<TEntity> where TEntity : class, IBaseModel, new()
    {
        IQueryable<TEntity> GetEntitiesQueryable(bool includeRemoved = false, bool skipApplicationCheck = false);
    }
}
