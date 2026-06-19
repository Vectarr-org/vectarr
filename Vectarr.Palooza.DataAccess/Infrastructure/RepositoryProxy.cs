using Vectarr.Palooza.Model;
using LinqKit.Core;
using System.Linq.Expressions;
using Vectarr.Palooza.DataAccess.IInfrastructure;

namespace Vectarr.Palooza.DataAccess.Infrastructure
{
    internal class RepositoryProxy<TEntity> : IRepositoryProxy<TEntity> where TEntity : class, IBaseModel, new()
    {
        private readonly IUnitOfWork _unitOfWork;

        public RepositoryProxy(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IQueryable<TEntity> GetEntitiesQueryable(bool includeRemoved = false)
        {
            var query = _unitOfWork.GetGenericRepository<TEntity>().GetQueryable(includeRemoved).AsExpandable();
            return query;
        }

        public IQueryable<TEntity> GetEntitiesQueryableWithInclude(bool includeRemoved = false)
        {
            var query = _unitOfWork.GetGenericRepository<TEntity>().GetQueryable(includeRemoved);
            return query;
        }

        public TEntity Insert(TEntity entity)
        {
            return _unitOfWork.GetGenericRepository<TEntity>().Insert(entity);
        }

        public void Update(TEntity entity)
        {
            _unitOfWork.GetGenericRepository<TEntity>().Update(entity);
        }

        public void Update(List<TEntity> entities)
        {
            _unitOfWork.GetGenericRepository<TEntity>().Update(entities);
        }

        public void Delete(TEntity entity)
        {
            _unitOfWork.GetGenericRepository<TEntity>().Delete(entity);
        }

        public void Delete(List<TEntity> entities)
        {
            _unitOfWork.GetGenericRepository<TEntity>().Delete(entities);
        }
    }
}
