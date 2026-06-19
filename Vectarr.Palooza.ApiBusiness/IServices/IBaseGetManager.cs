using Vectarr.Palooza.Classes.BaseApi;
using Vectarr.Palooza.Classes.Filtering;
namespace Vectarr.Palooza.ApiBusiness.IServices
{
    public interface IBaseGetManager<T> where T : class, IApiModel, new()
    {
        T GetItem(int id, bool loadExtendedData = true);
        List<T> GetItems(List<int> ids, bool loadExtendedData = true);
        bool AnyData(List<Filter> filters);
        List<T> Get(int pageNumber, int countPerPage, string orderBy, bool orderByDescending, out int totalCount, List<Filter> filters, bool loadExtendedData = true, bool skipApplicationCheck = false);
    }
}
