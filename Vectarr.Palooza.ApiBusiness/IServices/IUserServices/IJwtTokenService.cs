using Vectarr.Palooza.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Vectarr.Palooza.ApiBusiness.IServices.IUserServices
{
    public interface IJwtTokenService
    {
        string GenerateJwtToken(UserAccount user);
        ClaimsPrincipal ValidateToken(string jwtToken);
    }
}
