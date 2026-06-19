using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Vectarr.Palooza.Digifabster.Classes
{
    public class RegisterClientRequest
    {
        [JsonPropertyName("email")]
        public required string Email { get; set; }

        [JsonPropertyName("password")]
        public required string Password { get; set; }

        [JsonPropertyName("silent_login_enabled")]
        public bool SilentLoginEnabled { get; set; }

        [JsonPropertyName("send_notification")]
        public bool SendNotification { get; set; }

        [JsonPropertyName("nane")]
        public string Name { get; set; }

        [JsonPropertyName("surname")]
        public string Surname { get; set; }
    }
}
