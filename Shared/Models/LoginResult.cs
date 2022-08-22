using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SmartChat.Shared.Models
{
    public class LoginResult
    {
        [JsonPropertyName("Successful")]
        public bool Successful { get; set; }
        [JsonPropertyName("Error")]
        public string Error { get; set; }
        [JsonPropertyName("Token")]
        public string Token { get; set; }
    }
}
