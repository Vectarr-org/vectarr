using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.Model;
using System.Linq.Expressions;
using LinqKit;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.DataAccess.IRepositories;
using Vectarr.Palooza.DataAccess.Infrastructure;

namespace Vectarr.Palooza.DataAccess.Repositories
{
    public class UserAccountRepository : BaseRepository<UserAccount, UserAccountModel>, IUserAccountRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserAccountRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public UserAccountModel FindUserByEmail(string email)
        {
            return QueryItem(w => w.Email == email, skipApplicationCheck: true);
        }

        public override Expression<Func<UserAccount, UserAccountModel>> GetEntityToModelExpression()
        {
            return entity => new UserAccountModel()
            {
                Id = entity.Id,
                UserName = entity.UserName,
                EmailAddress = entity.Identity.EmailAddress,
                ApplicationId = entity.Application.Id,
                IdentityId = entity.Identity.Id,
                ReferralId = entity.ReferralId,
                ReferralCode = entity.ReferralCode,
                WorkshopId = entity.Workshop != null ? entity.Workshop.Id : null,
                CreatorId = entity.Creator != null ? entity.Creator.Id : null,
                DigifabsterId = entity.DigifabsterId,
            };
        }

        public UserAccount? GetByReferralCode(string referralCode)
        {
            return GetEntitiesQueryable().Where(w => w.ReferralCode != null && w.ReferralCode != "" &&
            w.ReferralCode.Equals(referralCode)).FirstOrDefault();
        }

        protected override UserAccount ModelToEntityInternal(UserAccountModel model, UserAccount entity)
        {
            entity.Identity = entity.Identity ?? UnitOfWork.Repositories.IdentityRepository.GetEntityById(model.IdentityId);
            entity.Application = entity.Application ?? UnitOfWork.Repositories.ApplicationRepository.GetEntityById(model.ApplicationId);
            entity.DigifabsterId = model.DigifabsterId;
            return entity;
        }

        public UserAccountModel GetUserByApplicationId(int applicationId)
        {
            return QueryItem(w => w.Application.Id == applicationId, skipApplicationCheck: true);
        }
    }
}
