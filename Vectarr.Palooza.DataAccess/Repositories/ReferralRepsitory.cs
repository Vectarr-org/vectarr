using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.Model;
using System.Linq.Expressions;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.DataAccess.IInfrastructure;
using Vectarr.Palooza.DataAccess.Infrastructure;
using Vectarr.Palooza.DataAccess.IRepositories;

namespace Vectarr.Palooza.DataAccess.Repositories
{
    public class ReferralRepsitory : BaseRepository<Referral, ReferralModel>, IReferralRepsitory, IApplicationRepository<Referral>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ReferralRepsitory(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Expression<Func<Referral, bool>> ApplicationPredicate(int applicationId)
        {
            return entity => entity.Application.Id == applicationId;
        }

        public ReferralModel GetByReferrerId(int referralId)
        {
            return QueryItem(w => w.ReferredUserId == referralId);
        }

        public override Expression<Func<Referral, ReferralModel>> GetEntityToModelExpression()
        {
            return entity => new ReferralModel()
            {
                Id = entity.Id,
                ReferredUserId = entity.Id,
                ReferralUserName = entity.ReferredUser.UserName,
                DateCreated = entity.CreatedDate,
                FreeTubEarned = entity.FreeTubEarned,
                ReferralEmail = entity.Application.User.Email,
            };
        }

        public List<ReferralModel> GetNonRewardedReferrals()
        {
            return GetEntitiesQueryable().Where(w => !w.FreeTubEarned).Select(GetEntityToModelExpression()).ToList();
        }

        protected override Referral ModelToEntityInternal(ReferralModel model, Referral entity)
        {
            entity.Application = entity.Application ?? Application;
            entity.ReferredUser = _unitOfWork.Repositories.UserAccountRepository.GetEntityById(model.ReferredUserId, true);
            entity.FreeTubEarned = model.FreeTubEarned;
            return entity;
        }
    }
}
