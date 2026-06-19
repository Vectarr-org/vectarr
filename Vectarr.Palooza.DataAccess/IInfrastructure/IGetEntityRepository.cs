using Vectarr.Palooza.Classes.BaseApi;
using Vectarr.Palooza.Model;
using System.Linq.Expressions;

namespace Vectarr.Palooza.DataAccess.IInfrastructure
{
    public interface IGetEntityRepository<TEntity, TModel> : IGetRepository<TModel>, ISimpleGetEntityRepository<TEntity> where TModel : class, IApiModel, new()
      where TEntity : class, IBaseModel, new()
    {
        TEntity GetEntityById(int? id, bool skipApplicationCheck = false);

        Expression<Func<TEntity, TModel>> GetEntityToModelExpression();
        List<TEntity> GetEntitiesById(List<int> idList, bool includeRemoved = false, bool skipApplicationCheck = false);
    }
}
