using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAZTLClient.Models
{
    public class RegisterRequest
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public required string First_Name { get; set; }
        public required string Last_Name { get; set; }
        public required string Role { get; set; }
    }
}
