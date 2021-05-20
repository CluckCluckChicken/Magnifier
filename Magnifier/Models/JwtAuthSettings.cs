using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magnifier.Models
{
    public class JwtAuthSettings : IJwtAuthSettings
    {
        public string PrivateKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public double LifetimeDays { get; set; }
    }

    public interface IJwtAuthSettings
    {
        string PrivateKey { get; set; }
        string Issuer { get; set; }
        string Audience { get; set; }
        double LifetimeDays { get; set; }
    }
}
