using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartChat.Shared.ViewModels
{
    public class SmartContact
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public long ConversationId { get; set; }
    }
}
