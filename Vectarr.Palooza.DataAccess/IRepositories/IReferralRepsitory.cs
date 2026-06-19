using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.DataAccess.IInfrastructure;

namespace Vectarr.Palooza.DataAccess.IRepositories
{
    public interface IReferralRepsitory : IEntityRepository<Referral, ReferralModel>
    {
        ReferralModel GetByReferrerId(int referralId);
        List<ReferralModel> GetNonRewardedReferrals();
    }
}
