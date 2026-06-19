using Vectarr.Palooza.Classes.BaseApi;

namespace Vectarr.Palooza.DataAccess.IInfrastructure
{
    public interface IRepository<TModel> : IGetRepository<TModel> where TModel : class, IApiModel, new()
    {
        TModel Update(TModel model);
        TModel UpdateWithInclude(TModel model, bool skipApplicationCheck = false);
        void Update(List<TModel> models);
        TModel Insert(TModel model);

        void Delete(TModel model, bool softDelete = true);
        void Delete(int id, bool softDelete = true);
        void Delete(List<TModel> models, bool softDelete = true);
        void Delete(List<int> ids, bool softDelete = true);
    }
}
