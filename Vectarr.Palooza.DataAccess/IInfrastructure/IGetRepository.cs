using Vectarr.Palooza.Classes.BaseApi;
using Vectarr.Palooza.Classes.Filtering;

namespace Vectarr.Palooza.DataAccess.IInfrastructure
{
    public interface IGetRepository<TModel> where TModel : class, IApiModel, new()
    {
        TModel GetById(int id, bool skipApplicationCheck = false);
        List<TModel> GetModelsByIds(List<int> idList);
        bool AnyData(List<Filter> filters = null);
        List<TModel> GetAll(bool skipApplicationCheck = false);
        List<TModel> GetAll(int pageNumber, int countPerPage, out int totalCount, List<Filter> filters = null, string orderBy = null, bool orderByDescending = false, bool skipApplicationCheck = false);
    }
}
