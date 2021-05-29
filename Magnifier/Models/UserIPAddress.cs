using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Magnifier.Models
{
    public class UserIPAddress
    {
        public UserIPAddress(IPAddress _ipAddress)
        {
            ipAddress = _ipAddress;

            time = DateTime.Now;
        }

        public IPAddress ipAddress { get; set; }

        public DateTime time { get; set; }
    }
}
