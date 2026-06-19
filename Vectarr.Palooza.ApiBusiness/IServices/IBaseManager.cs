using Vectarr.Palooza.Classes.BaseApi;

namespace Vectarr.Palooza.ApiBusiness.IServices
{
    public interface IBaseManager<T> : IBaseGetManager<T> where T : class, IApiModel, new()
    {
        bool DoModelExtraValidation(T model, out List<string> errors);
        T Insert(T item, bool doValidation = false);
        T Save(T item, bool doValidation = false, bool saveInternal = false);
        T Update(T item, bool doValidation = false, bool saveInternal = false);
        bool Validate(T model, out List<string> errors);
        void ThrowIfModelInvalid(T model);
    }
}
