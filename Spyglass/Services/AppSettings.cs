using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spyglass.Services
{
    public class AppSettings
    {
        public AppSettings()
        {
            ApiBase = $"{ApiRoot}/api";
        }

        public string ApiRoot { get; set; }

        public string ApiBase { get; set; }
    }
}
