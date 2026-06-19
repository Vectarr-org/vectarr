using Vectarr.Palooza.ApiBusiness.IServices;
using Vectarr.Palooza.Classes.BaseApi;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.DataAccess.IInfrastructure;
using Vectarr.Palooza.Exceptions;

namespace Vectarr.Palooza.WebApi.Controllers
{
    public abstract class BaseController<T> : GetController<T> where T : class, IApiModel, new()
    {
        protected BaseController(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, httpContextAccessor)
        {

        }

        protected abstract IBaseManager<T> GetManager();

        protected override IBaseGetManager<T> GetOnlyManager()
        {
            return GetManager();
        }

        protected void ValidateModel(T model)
        {
            List<string> errors;
            bool isModelValid = GetManager().DoModelExtraValidation(model, out errors);
            if (!(ModelState.IsValid && isModelValid))
            {
                errors = errors ?? new List<string>();
                errors.AddRange(ModelState.Values.SelectMany(x => x.Errors).Select(x => string.IsNullOrWhiteSpace(x.ErrorMessage) ? x.Exception?.Message : x.ErrorMessage).Distinct());
                var fullErrorsMessage = string.Format("Model Validation failed because of the following errors: {0}", string.Join("\n", errors));
                throw new ErrorValidationException(fullErrorsMessage);
            }
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("Save")]
        public virtual T Save(T item, bool saveInternal = false)
        {
            return GetManager().Save(item, true, saveInternal);
        }
    }
}
