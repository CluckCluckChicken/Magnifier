using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Magnifier.Models
{
    public record User
    {
        public User(string username, ScratchCommentAuthor scratchUser, bool isAdmin, DateTime created, DateTime lastLogin)
        {
            this.username = username;
            this.scratchUser = scratchUser;
            this.isAdmin = isAdmin;
            this.created = created;
            this.lastLogin = lastLogin;
        }

        public string id { get; set; }

        public string username { get; set; } // This user's Scratch username

        public ScratchCommentAuthor scratchUser { get; set; } // This user's Scratch profile

        public bool isAdmin { get; set; } // Is this user an admin?

        public DateTime created { get; set; } // When this user's account was created
        public DateTime lastLogin { get; set; } // When this user last logged into their account
    }
}