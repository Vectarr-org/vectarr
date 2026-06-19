using System.Linq.Expressions;
using LinqKit;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.Model;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.DataAccess.Infrastructure;
using Vectarr.Palooza.DataAccess.IRepositories;

namespace Vectarr.Palooza.DataAccess.Repositories
{
    public class CreatorRepository : BaseRepository<Creator, CreatorModel>, ICreatorRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreatorRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override Expression<Func<Creator, CreatorModel>> GetEntityToModelExpression()
        {
            var imageExpression = _unitOfWork.Repositories.ImageRepository.GetEntityToModelExpression().Expand();
            var addressExpression = _unitOfWork.Repositories.AddressRepository.GetEntityToModelExpression().Expand();
            return entity => new CreatorModel()
            {
                Id = entity.Id,
                Bio = entity.Bio,
                Company = entity.Company,
                Address = entity.Address != null ? addressExpression.Invoke(entity.Address) : null,
                ProfilePicture = entity.ProfilePicture != null ? imageExpression.Invoke(entity.ProfilePicture) : null,
            };
        }

        protected override Creator ModelToEntityInternal(CreatorModel model, Creator entity)
        {
            entity.Bio = model.Bio;
            entity.Company = model.Company;
            var profilePicture = entity.ProfilePicture;
            profilePicture = _unitOfWork.Repositories.ImageRepository.GetEntityById(model.ProfilePicture?.Id);
            entity.ProfilePicture = profilePicture;


            if (model.Address != null)
            {
                if (model.Address.Id != 0)
                {
                    // Get existing address and UPDATE it with new values from model
                    var existingAddress = _unitOfWork.Repositories.AddressRepository.GetEntityById(model.Address.Id);
                    entity.Address = _unitOfWork.Repositories.AddressRepository.ModelToEntity(model.Address, existingAddress);
                }
                else
                {
                    // Create new address
                    entity.Address = _unitOfWork.Repositories.AddressRepository.ModelToEntity(model.Address, new Address());
                }
            }
            else
            {
                entity.Address = null;
            }


            return entity;
        }
    }
}
