using System.Linq.Expressions;
using Vectarr.Palooza.Model;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.CoreClasses.Enums;
using Vectarr.Palooza.DataAccess.IInfrastructure;
using Vectarr.Palooza.DataAccess.IRepositories;
using Vectarr.Palooza.DataAccess.Infrastructure;

namespace Vectarr.Palooza.DataAccess.Repositories
{
    public class ApplicationSubscriptionRepository : BaseRepository<ApplicationSubscription, ApplicationSubscriptionModel>, IApplicationSubscriptionRepository, IApplicationRepository<ApplicationSubscription>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ApplicationSubscriptionRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Expression<Func<ApplicationSubscription, bool>> ApplicationPredicate(int applicationId)
        {
            return entity => entity.Application.Id == applicationId && entity.SubscriptionStatus != SubscriptionStatusEnum.Canceled;
        }

        public override Expression<Func<ApplicationSubscription, ApplicationSubscriptionModel>> GetEntityToModelExpression()
        {
            return entity => new ApplicationSubscriptionModel()
            {
                Id = entity.Id,
                BillingCycleStartDate = entity.BillingCycleStartDate,
                DateCreated = entity.CreatedDate,
                PaymentsFailureStartDate = entity.PaymentsFailureStartDate,
                SubscriptionPrice = entity.SubscriptionPrice,
                Subscription = entity.Subscription,
                SubscriptionStatus = entity.SubscriptionStatus,
                CancelReason = entity.CancelReason,
                UserEmail = entity.Application.User.Email,
            };
        }

        protected override ApplicationSubscription ModelToEntityInternal(ApplicationSubscriptionModel model, ApplicationSubscription entity)
        {
            entity.Application = entity.Application ?? Application;
            entity.BillingCycleStartDate = model.BillingCycleStartDate;
            entity.PaymentsFailureStartDate = model.PaymentsFailureStartDate;
            entity.SubscriptionPrice = model.SubscriptionPrice;
            entity.Subscription = model.Subscription;
            entity.SubscriptionStatus = model.SubscriptionStatus;
            entity.CancelReason = model.CancelReason;


            return entity;
        }

        public ApplicationSubscriptionModel GetSubscription()
        {
            return QueryItem();
        }
    }
}
