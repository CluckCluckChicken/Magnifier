using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magnifier.Models
{
    public record AuthCode
    {
        public AuthCode(string _code, bool _hasBeenUsed = false)
        {
            code = _code;
            hasBeenUsed = _hasBeenUsed;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public string id { get; set; }

        public string code { get; set; }

        public bool hasBeenUsed { get; set; }
    }
}
