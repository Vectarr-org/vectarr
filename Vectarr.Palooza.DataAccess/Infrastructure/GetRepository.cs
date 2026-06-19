using LinqKit;
using LinqKit.Core;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Vectarr.Palooza.Model;
using Vectarr.Palooza.Classes.BaseApi;
using Microsoft.EntityFrameworkCore;
using Vectarr.Palooza.Classes.Filtering;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.DataAccess.IInfrastructure;

namespace Vectarr.Palooza.DataAccess.Infrastructure
{
    public abstract class GetRepository<TEntity, TModel> : IGetEntityRepository<TEntity, TModel>
      where TModel : class, IApiModel, new()
      where TEntity : class, IBaseModel, new()
    {
        protected int ApplicationId
        {
            get { return UnitOfWork.ApplicationId; }
        }

        public virtual bool AnyData(List<Filter> filters = null)
        {
            return GetEntitiesQueryable().Any(w => !w.IsHidden);
        }


        public virtual List<TModel> GetAll(int pageNumber, int countPerPage, out int totalCount,
    List<Filter> filters = null, string orderBy = null, bool orderByDescending = false, bool skipApplicationCheck = false)
        {
            if (pageNumber < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageNumber), "The page number should be 1 or higher");
            }

            var query = GetEntitiesQueryable(skipApplicationCheck: skipApplicationCheck).AsNoTracking();

            filters?.ForEach(filter => query = Filter(query, filter));

            query = ApplyPaging(query, pageNumber, countPerPage, out totalCount, orderBy, orderByDescending);
            var result = query.Select(GetEntityToModelExpression().Expand()).ToList();
            return result;
        }

        public IQueryable<TEntity> ApplyPaging(IQueryable<TEntity> query, int pageNumber, int countPerPage, out int totalCount,
        string orderBy = null, bool orderByDescending = false)
        {
            if (pageNumber < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageNumber), "The page number should be 1 or higher");
            }

            totalCount = query.Count();

            if (!string.IsNullOrEmpty(orderBy))
            {
                query = orderByDescending ? query.OrderBy(orderBy + " descending") : query.OrderBy(orderBy);
            }
            else
            {
                query = query.OrderBy(GetDefaultOrderByProperty());
            }
            var skipPages = (pageNumber - 1) * countPerPage;
            query = query.AsExpandable().Skip(skipPages)
                .Take(countPerPage);

            return query;
        }

        public List<TModel> GetAll(bool skipHidden = false, bool skipApplicationCheck = false)
        {
            var query = GetEntitiesQueryable(skipApplicationCheck: skipApplicationCheck).AsNoTracking();
            var result = query.Select(GetEntityToModelExpression().Expand()).ToList();
            return result;
        }

        protected virtual Expression<Func<TEntity, int>> GetDefaultOrderByProperty()
        {
            return entity => entity.Id;
        }

        public virtual IQueryable<TEntity> Filter(IQueryable<TEntity> query, Filter filter)
        {
            return query;
        }

        protected readonly IUnitOfWork UnitOfWork;

        protected GetRepository(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        private Application _application;

        protected Application Application
        {
            get { return _application ?? (_application = UnitOfWork.Repositories.ApplicationRepository.GetEntityById(ApplicationId)); }
        }

        private IRepositoryProxy<TEntity> _repositoryProxy;

        internal IRepositoryProxy<TEntity> RepositoryProxy
        {
            get
            {
                if (_repositoryProxy == null)
                {
                    _repositoryProxy = new RepositoryProxy<TEntity>(UnitOfWork);
                }
                return _repositoryProxy;
            }
        }

        public List<TModel> GetAll(bool skipApplicationCheck = false)
        {
            var query = GetEntitiesQueryable(skipApplicationCheck: skipApplicationCheck).AsNoTracking();
            var result = query.Select(GetEntityToModelExpression().Expand()).ToList();
            return result;
        }

        public IQueryable<TEntity> GetEntitiesQueryable(bool includeRemoved = false, bool skipApplicationCheck = false)
        {
            var query = RepositoryProxy.GetEntitiesQueryable(includeRemoved);

            if (!skipApplicationCheck && this is IApplicationRepository<TEntity>)
            {
                var applicationPredicate =
                    ((IApplicationRepository<TEntity>)this).ApplicationPredicate(ApplicationId);
                query = query.Where(applicationPredicate);
            }

            return query;
        }

        public IQueryable<TEntity> GetEntitiesQueryableWithInclude(bool includeRemoved = false, bool skipApplicationCheck = false)
        {
            var query = RepositoryProxy.GetEntitiesQueryableWithInclude(includeRemoved);

            query = GetIncludes(query).AsExpandable();

            if (!skipApplicationCheck && this is IApplicationRepository<TEntity>)
            {
                var applicationPredicate =
                    ((IApplicationRepository<TEntity>)this).ApplicationPredicate(ApplicationId);
                query = query.Where(applicationPredicate);
            }

            return query;
        }

        public TEntity GetEntityById(int? id, bool skipApplicationCheck = false)
        {
            if (!id.HasValue)
            {
                return null;
            }

            return GetEntitiesQueryable(skipApplicationCheck: skipApplicationCheck).FirstOrDefault(entity => entity.Id == id);
        }

        public TEntity GetEntityByIdWithInclude(int? id, bool skipApplicationCheck = false)
        {
            if (!id.HasValue)
            {
                return null;
            }

            var query = GetEntitiesQueryableWithInclude(false, skipApplicationCheck: skipApplicationCheck);

            return query.FirstOrDefault(entity => entity.Id == id);
        }

        public virtual IQueryable<TEntity> GetIncludes(IQueryable<TEntity> query)
        {
            return query;
        }

        public TModel GetById(int id, bool skipApplicationCheck = false)
        {
            return QueryItem(x => x.Id == id, skipApplicationCheck: skipApplicationCheck);
        }

        public List<TEntity> GetEntitiesById(List<int> idList, bool includeRemoved = false, bool skipApplicationCheck = false)
        {
            if (idList == null)
            {
                return new List<TEntity>();
            }

            var builder = new PredicateGenerator<TEntity, int>();
            var predicate = builder.Create(idList, x => x.Id);
            return GetEntitiesQueryable(includeRemoved, skipApplicationCheck).Where(predicate).ToList();
        }

        public abstract Expression<Func<TEntity, TModel>> GetEntityToModelExpression();

        protected TModel QueryItem(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IQueryable<TEntity>> orderByFunc = null, bool includeRemoved = false, bool skipApplicationCheck = false)
        {
            if (predicate == null)
            {
                predicate = (x) => true;
            }

            var query = GetEntitiesQueryable(includeRemoved, skipApplicationCheck)
               .Where(predicate);

            if (orderByFunc != null)
            {
                query = orderByFunc(query);
            }

            var model = query
                .Select(GetEntityToModelExpression().Expand())
                .FirstOrDefault();


            return model;
        }

        public List<TModel> GetModelsByIds(List<int> idList)
        {
            if (idList == null || !idList.Any())
            {
                return new List<TModel>();
            }

            List<TModel> result = new List<TModel>();
            idList = idList.Distinct().ToList();

            foreach (var id in idList)
            {
                var item = GetById(id);
                if (item != null)
                {
                    result.Add(item);
                }
            }

            return result;
        }
    }
}
