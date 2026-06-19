using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Vectarr.Palooza.Digifabster.Classes
{
    public class RegisterClientResponse
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("silent_login_enabled")]
        public bool? SilentLoginEnabled { get; set; }

        [JsonExtensionData]
        public Dictionary<string, object>? AdditionalData { get; set; }
    }
}
