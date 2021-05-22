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
        public ScratchComment(int _id, string _content, ScratchCommentAuthor _author, DateTime? _datetime_created)
        {
            id = _id;
            content = _content;
            datetime_created = _datetime_created;
            author = _author;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        public int id { get; set; }

        public string content { get; set; }

        public DateTime? datetime_created { get; set; }

        public ScratchCommentAuthor author { get; set; }
    }
}