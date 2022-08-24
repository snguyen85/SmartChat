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
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; }
    }
}
