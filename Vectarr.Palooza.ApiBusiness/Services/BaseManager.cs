using System.ComponentModel.DataAnnotations;
using Vectarr.Palooza.Classes.BaseApi;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.DataAccess.IInfrastructure;
using Vectarr.Palooza.Exceptions;
using Vectarr.Palooza.ApiBusiness.IServices;

namespace Vectarr.Palooza.ApiBusiness.Services
{
    public abstract class BaseManager<T> : BaseGetManager<T>, IBaseManager<T> where T : class, IApiModel, new()
    {

        public BaseManager(IUnitOfWork unitOfWork, IRepository<T> repository) : base(unitOfWork, repository)
        {
        }

        private IRepository<T> Repository
        {
            get { return (IRepository<T>)GetOnlyRepository; }
        }

        public bool Validate(T model, out List<string> errors)
        {
            var validationContext = new ValidationContext(model);
            List<ValidationResult> validationErrors = new List<ValidationResult>();
            errors = new List<string>();
            bool isValid = Validator.TryValidateObject(model, validationContext, validationErrors, true);
            isValid = isValid && DoModelExtraValidation(model, out errors);
            errors.AddRange(validationErrors.Select(e => e.ErrorMessage));
            return isValid;
        }

        public void ThrowIfModelInvalid(T model)
        {
            List<string> errors;
            bool isModelValid = Validate(model, out errors);
            if (!isModelValid)
            {
                var fullErrorsMessage = string.Join("\n", errors);
                throw new ErrorValidationException(fullErrorsMessage);
            }
        }

        public virtual bool DoModelExtraValidation(T model, out List<string> errors)
        {
            errors = new List<string>();
            return true;
        }

        public T Save(T item, bool doValidation = false, bool saveInternal = false)
        {
            return item.Id == 0 ? Insert(item, doValidation) : Update(item, doValidation, saveInternal);
        }

        public T Insert(T item, bool doValidation = false)
        {
            if (doValidation)
            {
                ThrowIfModelInvalid(item);
            }

            var model = InsertInternal(item);
            return model;
        }

        public T Update(T item, bool doValidation = false, bool saveInternal = false)
        {
            if (doValidation)
            {
                ThrowIfModelInvalid(item);
            }

            if (saveInternal)
            {
                return UpdateIncludeInternal(item);
            }
            else
            {
                return UpdateInternal(item);
            }
        }

        protected virtual T InsertInternal(T item)
        {
            var model = Repository.Insert(item);
            return model;
        }

        protected virtual T UpdateInternal(T item)
        {
            var model = Repository.Update(item);
            return model;
        }

        protected virtual T UpdateIncludeInternal(T item)
        {
            var model = Repository.UpdateWithInclude(item);
            return model;
        }

        public virtual void Delete(T item)
        {
            Repository.Delete(item);

        }

        public virtual void Delete(List<T> items)
        {
            Repository.Delete(items);

        }

        public virtual void Delete(int id)
        {
            var item = GetItem(id);
            Repository.Delete(id);
        }

        public virtual void Delete(List<int> ids)
        {
            Repository.Delete(ids);
        }
    }
}
