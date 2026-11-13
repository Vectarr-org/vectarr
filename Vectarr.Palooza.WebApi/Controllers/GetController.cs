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
        [Route("Get")]
        public virtual ItemViewModel<T> GetItem(int id)
        {
            return GetItemInternal(GetOnlyManager(), id);
        }

        [HttpGet]
        [Route("BatchGet")]
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
            return new ListViewModel<TModel>(data, count);
        }

        [HttpGet]
        [Route("List")]
        public virtual ListViewModel<T> Get(int pageNumber = 1, int countPerPage = 10, string? orderBy = null, string? sort = null)
        {
            var filters = GetFilters();
            return GetInternal(GetOnlyManager(), pageNumber, countPerPage, filters, orderBy,
                sort != null && sort.StartsWith("d"));
        }

        protected abstract IBaseGetManager<T> GetOnlyManager();
    }
}
