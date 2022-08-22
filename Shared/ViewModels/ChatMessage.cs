using SmartChat.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartChat.Shared.ViewModels
{
    public class ChatMessage
    {
        public long Id { get; set; }
        public long ConversationId { get; set; }
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; }
        public virtual ApplicationUser FromUser { get; set; }
        public virtual ApplicationUser ToUser { get; set; }
    }
}
