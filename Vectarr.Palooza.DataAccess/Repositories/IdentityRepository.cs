using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.Model;
using LinqKit;
using System.Linq.Expressions;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.DataAccess.IRepositories;
using Vectarr.Palooza.DataAccess.Infrastructure;

namespace Vectarr.Palooza.DataAccess.Repositories
{
    public class IdentityRepository : BaseRepository<Identity, IdentityModel>, IIdentityRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public IdentityRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override Expression<Func<Identity, IdentityModel>> GetEntityToModelExpression()
        {
            var imageExpression = _unitOfWork.Repositories.ImageRepository.GetEntityToModelExpression().Expand();
            var addressExpression = _unitOfWork.Repositories.AddressRepository.GetEntityToModelExpression().Expand();
            return entity => new IdentityModel()
            {
                Id = entity.Id,
                EmailAddress = entity.EmailAddress,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                ProfilePicture = entity.ProfilePicture != null ? imageExpression.Invoke(entity.ProfilePicture) : new ImageModel(),
                Address = entity.Address != null ? addressExpression.Invoke(entity.Address) : new AddressModel() { Country = CountryEnum.UnitedStates },
                Age = entity.Age,
                PhoneNumber = entity.PhoneNumber,
                IsEmailSubscribed = entity.IsEmailSubscribed,
                TaxRate = entity.TaxRate,

            };
        }

        protected override Identity ModelToEntityInternal(IdentityModel model, Identity entity)
        {
            entity.EmailAddress = model.EmailAddress;
            entity.FirstName = model.FirstName;
            entity.Age = model.Age;
            entity.PhoneNumber = model.PhoneNumber;
            entity.LastName = model.LastName;
            entity.TaxRate = model.TaxRate;
            entity.IsEmailSubscribed = model.IsEmailSubscribed;
            entity.Address = model.Address != null && model.Address.Id != 0 ?
                           _unitOfWork.Repositories.AddressRepository.ModelToEntity(model.Address, _unitOfWork.Repositories.AddressRepository.GetEntityById(model.Address.Id)) :
                            null;

            var profilePicture = entity.ProfilePicture;
            profilePicture = UnitOfWork.Repositories.ImageRepository.GetEntityById(model.ProfilePicture?.Id);
            entity.ProfilePicture = profilePicture;


            return entity;
        }

        public IdentityModel GetCurrentUserIdentity()
        {
            return GetById(_unitOfWork.CurrentUser.Identity.Id);
        }
    }
}
