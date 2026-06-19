using Vectarr.Palooza.Model;
using System.Linq.Expressions;

namespace Vectarr.Palooza.DataAccess.IInfrastructure
{
    public interface IApplicationRepository<TEntity> where TEntity : IBaseModel
    {
        Expression<Func<TEntity, bool>> ApplicationPredicate(int applicationId);
    }
}
