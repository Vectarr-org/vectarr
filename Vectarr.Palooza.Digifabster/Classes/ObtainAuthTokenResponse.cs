using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Vectarr.Palooza.Digifabster.Classes
{
    internal class ObtainAuthTokenResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;
    }
}
