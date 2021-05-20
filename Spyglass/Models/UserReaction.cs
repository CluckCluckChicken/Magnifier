using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magnifier.Models
{
    public record UserReaction
    {
        public UserReaction(string id, string user, string reaction, DateTime timeReacted)
        {
            this.id = id;
            this.user = user;
            this.reaction = reaction;
            this.timeReacted = timeReacted;
        }

        public string id { get; set; }

        public string user { get; set; }

        public string reaction { get; set; }

        public DateTime timeReacted { get; set; }
    }
}