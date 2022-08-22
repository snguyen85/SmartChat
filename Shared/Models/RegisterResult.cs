using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SmartChat.Shared.Models
{
    public class RegisterResult
    {
        [JsonPropertyName("Successful")]
        public bool Successful { get; set; }
        [JsonPropertyName("Errors")]
        public IEnumerable<string> Errors { get; set; }
    }
}
