using Vectarr.Palooza.Classes.BaseApi;
using Vectarr.Palooza.DataAccess.IInfrastructure.DeleteHandler;
using Vectarr.Palooza.Model;

namespace Vectarr.Palooza.DataAccess.IInfrastructure
{
    public interface IEntityRepository<TEntity, TModel> : IRepository<TModel>, IGetEntityRepository<TEntity, TModel> where TModel : class, IApiModel, new()
   where TEntity : class, IBaseModel, new()
    {
        TEntity ModelToEntity(TModel model, TEntity entity);
        List<TEntity> ReplaceCollection(IEnumerable<TEntity> original, List<TEntity> updated, bool softDelete = true);
        void Update(List<TEntity> entities);
        TEntity Insert(TEntity entity);
        TModel UpdateWithInclude(TModel model, bool skipApplicationCheck = false);
        DeleteHandler<TEntity> GetDeleteHandler();
        void Delete(List<TEntity> entities, bool softDelete = true);
        void Delete(TEntity entity, bool softDelete = true);
        bool ShouldHideInsteadOfDelete(TEntity entity);
    }
}
