using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magnifier.Models
{
    public record UserReaction
    {
        public UserReaction(string _user, string _reaction)
        {
            user = _user;
            reaction = _reaction;
            timeReacted = DateTime.Now;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public string id { get; set; }

        public string user { get; set; }

        public string reaction { get; set; }

        public DateTime timeReacted { get; set; }
    }
}