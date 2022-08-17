using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartChat.Shared.Models
{
    /// <summary>
    /// Direct messages between two users
    /// </summary>
    public class DirectMessage
    {
        public int Id { get; set; }
        // intended recipient
        public string ToUserId { get; set; }
        // author
        public string FromUserId { get; set; }
        public long MessageId { get; set; }
    }
}
