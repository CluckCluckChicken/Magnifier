using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magnifier.Models
{
    public record Reaction
    {
        public Reaction(string _name, string _emoji)
        {
            name = _name;
            emoji = _emoji;
        }

        public string id { get; set; }

        public string name { get; set; }

        public string emoji { get; set; }
    }
}