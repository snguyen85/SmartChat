using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartChat.Shared.Models
{
    public class Message
    {
        public long Id { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; }
    }
}
