using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.Model;
using System.Linq.Expressions;
using Vectarr.Palooza.DataAccess.Infrastructure;
using Vectarr.Palooza.DataAccess.IRepositories;

namespace Vectarr.Palooza.DataAccess.Repositories
{
    public class AddressRepository : BaseRepository<Address, AddressModel>, IAddressRepository
    {
        public AddressRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public override Expression<Func<Address, AddressModel>> GetEntityToModelExpression()
        {
            return entity => new AddressModel()
            {
                City = entity.City,
                Country = entity.Country,
                State = entity.State,
                Street1 = entity.Street1,
                Street2 = entity.Street2,
                PostalCode = entity.PostalCode,
                Id = entity.Id
            };
        }

        protected override Address ModelToEntityInternal(AddressModel model, Address entity)
        {
            entity.Street1 = model.Street1;
            entity.Street2 = model.Street2;
            entity.City = model.City;
            entity.Country = model.Country;
            entity.State = model.State;
            entity.PostalCode = model.PostalCode;
            return entity;
        }
    }
}
