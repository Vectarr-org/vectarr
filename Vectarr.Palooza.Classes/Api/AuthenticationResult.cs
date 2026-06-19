using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vectarr.Palooza.Classes.Api
{
    public class AuthenticationResult
    {
        public string Token { get; set; }
        public bool Success { get; set; }
        public List<string> Errors { get; set; }
        public int? IdentityId { get; set; }
        public bool EmailAlreadyRegistered { get; set; }
        public JobModel? Job { get; set; }
    }
}
