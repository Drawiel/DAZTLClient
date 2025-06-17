using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAZTLClient.Models
{
    public class MessageChatUnmarshalling
    {
        public string type { get; set; }
        public string username { get; set; }
        public string message { get; set; }
    }
}
