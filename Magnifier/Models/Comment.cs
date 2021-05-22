using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magnifier.Models
{
    public record Comment
    {
        public Comment(int _commentId, ScratchComment _comment, List<int> _replies)
        {
            commentId = _commentId;
            comment = _comment;
            replies = _replies;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        public int commentId { get; set; }

        public ScratchComment comment { get; set; }

        public List<UserReaction> reactions { get; set; }

        public bool isPinned { get; set; }

        public List<int> replies { get; set; }
    }
}