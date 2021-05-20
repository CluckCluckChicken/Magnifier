// This class currently does nothing. Go away.

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
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }

        public string uid { get; set; }

        public string username { get; set; }

        public List<UserIPAddress> ipAddresses { get; set; }
    }
}