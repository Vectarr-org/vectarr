using Vectarr.Palooza.Model;

namespace Vectarr.Palooza.DataAccess
{
    public interface IGenericRepository<T> where T : class, IBaseModel, new()
    {
        void Delete(T entity);
        void Delete(List<T> entities);
        T GetById(int? id);
        IQueryable<T> GetQueryable(bool includeRemoved = false);
        T Insert(T entity);
        void Update(T entity);
        void Update(List<T> entities);
    }
}
