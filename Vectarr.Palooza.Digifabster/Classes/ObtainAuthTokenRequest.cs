using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Vectarr.Palooza.Digifabster.Classes
{
    internal class ObtainAuthTokenRequest
    {
        [JsonPropertyName("company_name")]
        public required string CompanyName { get; init; }

        [JsonPropertyName("email")]
        public required string Email { get; init; }

        [JsonPropertyName("password")]
        public required string Password { get; init; }
    }
}
