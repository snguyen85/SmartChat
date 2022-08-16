using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartChat.Shared.Models
{
    /// <summary>
    /// Direct messages between two users
    /// </summary>
    public class Conversations
    {
        public int Id { get; set; }
        public int ToId { get; set; }
        public int FromId { get; set; }
        public int MessageId { get; set; }
    }
}
