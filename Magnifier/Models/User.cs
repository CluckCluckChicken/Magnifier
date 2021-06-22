using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Magnifier.Models
{
    public record User
    {
        public User(string _username, ScratchCommentAuthor _scratchUser, bool _isAdmin)
        {
            username = _username;
            scratchUser = _scratchUser;
            isAdmin = _isAdmin;

            created = DateTime.Now;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }

        // User info

        public string username { get; set; } // This user's Scratch username

        public ScratchCommentAuthor scratchUser { get; set; } // This user's Scratch profile

        public bool isAdmin { get; set; } // Is this user an admin?

        public bool isBanned { get; set; } // Is this user the sus imposter?

        public DateTime created { get; private set; } // When this user's account was created
        public DateTime lastLogin { get; set; } // When this user last logged into their account

        // Other stuff
        
        public Settings settings { get; set; }

        public List<int> stars { get; set; } // List of ids of the comments that this user has starred
    }
}