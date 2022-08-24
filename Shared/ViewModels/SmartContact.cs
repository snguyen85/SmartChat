using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SmartChat.Shared.ViewModels
{
    public class SmartContact
    {
        [JsonPropertyName("Id")]
        public string Id { get; set; }
        [JsonPropertyName("Username")]
        public string UserName { get; set; }
        [JsonPropertyName("ConversationId")]
        public long ConversationId { get; set; }
    }
}
