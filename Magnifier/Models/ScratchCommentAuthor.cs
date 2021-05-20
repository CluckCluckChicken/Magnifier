using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magnifier.Models
{
    public class ScratchCommentAuthor
    {
        public ScratchCommentAuthor(string _username, string _image)
        {
            username = _username;
            image = _image;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        public string username { get; set; }

        public string image { get; set; }
    }
}
