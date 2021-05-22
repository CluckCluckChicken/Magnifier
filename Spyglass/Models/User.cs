using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Magnifier.Models
{
    public record User
    {
        public User(string username, ScratchCommentAuthor scratchUser, bool isAdmin)
        {
            this.username = username;
            this.scratchUser = scratchUser;
            this.isAdmin = isAdmin;
            created = DateTime.Now;
        }

        public string id { get; set; }

        public string username { get; set; } // This user's Scratch username

        public ScratchCommentAuthor scratchUser { get; set; } // This user's Scratch profile

        public bool isAdmin { get; set; } // Is this user an admin?

        public DateTime created { get; private set; } // When this user's account was created
        public DateTime lastLogin { get; private set; } // When this user last logged into their account

        public List<UserIPAddress> ipAddresses { get; set; } // The IP addresses this user has logged in with
    }
}