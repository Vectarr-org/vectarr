using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.Model;
using System.Linq.Expressions;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.DataAccess.IInfrastructure;
using Vectarr.Palooza.DataAccess.Infrastructure;
using Vectarr.Palooza.DataAccess.IRepositories;

namespace Vectarr.Palooza.DataAccess.Repositories
{
    public class ApplicationRepository : BaseRepository<Application, ApplicationModel>, IAppRepository, IApplicationRepository<Application>
    {
        public ApplicationRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public Expression<Func<Application, bool>> ApplicationPredicate(int applicationId)
        {
            return entity => entity.Id == applicationId;
        }

        public override Expression<Func<Application, ApplicationModel>> GetEntityToModelExpression()
        {
            return entity => new ApplicationModel
            {
                Id = entity.Id,
            };
        }

        protected override Application ModelToEntityInternal(ApplicationModel model, Application entity)
        {

            return entity;
        }

        public int GetApplicationIdByUser(int userIdentityId)
        {
            return QueryItem(w => w.User.Identity.Id == userIdentityId, skipApplicationCheck: true).Id;
        }
    }
}
