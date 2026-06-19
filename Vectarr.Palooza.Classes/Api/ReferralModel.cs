using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.Classes.BaseApi;

namespace Vectarr.Palooza.Classes.Api
{
    public class ReferralModel : BaseApiModel
    {
        public int ReferredUserId { get; set; }
        public string? ReferralUserName { get; set; }
        public bool FreeTubEarned { get; set; }
        public string? ReferralEmail { get; set; }
    }
}
