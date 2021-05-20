using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magnifier.Models
{
    public class ScratchComment
    {
        public ScratchComment(int _id, string _content, ScratchCommentAuthor _author)
        {
            id = _id;
            content = _content;
            author = _author;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        public int id { get; set; }

        public string content { get; set; }

        public ScratchCommentAuthor author { get; set; }
    }
}
