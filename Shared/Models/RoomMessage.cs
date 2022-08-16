using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartChat.Shared.Models
{
    /// <summary>
    /// Chat room messages
    /// </summary>
    public class RoomMessage
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public int MessageId { get; set; }
    }
}
