using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartChat.Shared.Models
{
    public class RoomMembership
    {
        public int Id { get; set; }
        public int ApplicationUserId { get; set; }
        public int RoomId { get; set; }
    }
}
