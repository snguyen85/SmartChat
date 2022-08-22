using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartChat.Shared.Models
{
    public class UserConversation
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public long ConversationId { get; set; }
    }
}
