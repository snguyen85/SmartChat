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
    public class DirectMessage
    {
        public int Id { get; set; }
        // intended recipeient
        public int ToId { get; set; }
        // author
        public int FromId { get; set; }
        public int MessageId { get; set; }
    }
}
