using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DAZTLClient.Models
{
    public class NotificationUnmarshalling
    {
        public string type { get; set; }
        public int id { get; set; }
        public string message { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime created_at { get; set; }
    }
}
