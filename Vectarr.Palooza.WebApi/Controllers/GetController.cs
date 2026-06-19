using Microsoft.AspNetCore.Mvc;
using Vectarr.Palooza.Classes.BaseApi;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.ApiBusiness.IServices;
using Vectarr.Palooza.Classes.Filtering;

namespace Vectarr.Palooza.WebApi.Controllers
{
    [Route("api/[controller]")]
    public abstract class GetController<T> : SystemController where T : class, IApiModel, new()
    {
        public GetController(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, httpContextAccessor)
        {

        }

        [HttpGet]
        [Route("GetItem")]
        public virtual ItemViewModel<T> GetItem(int id)
        {
            return GetItemInternal(GetOnlyManager(), id);
        }

        [HttpGet]
        [Route("GetItems")]
        public virtual List<T> GetItems(string ids)
        {
            var idList = ids.Split(",").Select(int.Parse).ToList();
            return GetOnlyManager().GetItems(idList);
        }

        protected ItemViewModel<TModel> GetItemInternal<TModel>(IBaseGetManager<TModel> manager, int id)
            where TModel : class, IApiModel, new()
        {
            var item = manager.GetItem(id);
            return new ItemViewModel<TModel>(item);
        }

        protected ListViewModel<TModel> GetInternal<TModel>(IBaseGetManager<TModel> manager, int pageNumber = 1, int countPerPage = 10, List<Filter> filters = null, string orderBy = null, bool descending = false)
          where TModel : class, IApiModel, new()
        {
            int count;
            var data = manager.Get(pageNumber, countPerPage, orderBy, descending, out count, filters);
            var hasData = manager.AnyData(filters);
            var filtersDictionary = filters?.Select(e => e.SourceKeyValuePair).ToList();
            return new ListViewModel<TModel>(data, count)
            {
                HasData = hasData,
                SearchInfo = new SearchInfo
                {
                    FilterQueryString = filtersDictionary,
                    OrderBy = orderBy,
                    OrderByDescending = descending,
                    CountPerPage = countPerPage,
                    ReturnedIds = data.Select(e => e.Id).ToList(),
                    Page = pageNumber,
                    TotalPages = (int)Math.Ceiling((decimal)count / countPerPage)
                }
            };
        }

        [HttpGet]
        [Route("Get")]
        public virtual ListViewModel<T> Get(int pageNumber = 1, int countPerPage = 10, string? orderBy = null, string? sort = null)
        {
            var filters = GetFilters();
            return GetInternal(GetOnlyManager(), pageNumber, countPerPage, filters, orderBy,
                sort != null && sort.StartsWith("d"));
        }

        protected abstract IBaseGetManager<T> GetOnlyManager();
    }
}
