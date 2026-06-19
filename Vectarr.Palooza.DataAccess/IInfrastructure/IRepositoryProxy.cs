using Vectarr.Palooza.Model;
using System.Linq.Expressions;

namespace Vectarr.Palooza.DataAccess.IInfrastructure
{
    internal interface IRepositoryProxy<TEntity> where TEntity : IBaseModel, new()
    {
        IQueryable<TEntity> GetEntitiesQueryable(bool includeRemoved = false);
        IQueryable<TEntity> GetEntitiesQueryableWithInclude(bool includeRemoved = false);
        TEntity Insert(TEntity entity);
        void Update(TEntity entity);
        void Update(List<TEntity> entities);
        void Delete(TEntity entity);
        void Delete(List<TEntity> entities);
    }
}
