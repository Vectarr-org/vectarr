using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.DataAccess.IInfrastructure;
using Vectarr.Palooza.Classes.BaseApi;
using Vectarr.Palooza.Classes.Filtering;
using Vectarr.Palooza.ApiBusiness.IServices;

namespace Vectarr.Palooza.ApiBusiness.Services
{
    public abstract class BaseGetManager<T> : IBaseGetManager<T> where T : class, IApiModel, new()
    {
        protected readonly IUnitOfWork UnitOfWork;
        protected readonly IGetRepository<T> GetOnlyRepository;

        public BaseGetManager(IUnitOfWork unitOfWork, IGetRepository<T> getOnlyRepository)
        {
            UnitOfWork = unitOfWork;
            GetOnlyRepository = getOnlyRepository;
        }

        public virtual bool AnyData(List<Filter> filters)
        {
            return GetOnlyRepository.AnyData(filters);
        }

        public virtual List<T> Get(int pageNumber, int countPerPage, string orderBy, bool orderByDescending, out int totalCount, List<Filter> filters, bool loadExtendedData = true, bool skipApplicationCheck = false)
        {
            var data = GetOnlyRepository.GetAll(pageNumber, countPerPage, out totalCount, filters, orderBy, orderByDescending, skipApplicationCheck);
            return loadExtendedData ? LoadAdditionalInfo(data) : data;
        }


        public T GetItem(int id, bool loadExtendedData = true)
        {
            var item = GetOnlyRepository.GetById(id);
            return loadExtendedData ? LoadAdditionalInfo(item) : item;
        }

        public List<T> GetItems(List<int> ids, bool loadExtendedData = true)
        {
            var items = GetOnlyRepository.GetModelsByIds(ids);
            return items;
        }

        protected virtual List<T> LoadAdditionalInfo(List<T> data)
        {
            return data;
        }

        protected T LoadAdditionalInfo(T item)
        {
            return LoadAdditionalInfo(new List<T> { item }).First();
        }
    }
}
