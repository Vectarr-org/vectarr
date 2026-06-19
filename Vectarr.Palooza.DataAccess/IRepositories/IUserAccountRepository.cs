using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.Model;
using Vectarr.Palooza.DataAccess.IInfrastructure;

namespace Vectarr.Palooza.DataAccess.IRepositories
{
    public interface IUserAccountRepository : IEntityRepository<UserAccount, UserAccountModel>
    {
        UserAccountModel FindUserByEmail(string email);
        UserAccountModel GetUserByApplicationId(int applicationId);
        UserAccount? GetByReferralCode(string referralCode);
    }
}
