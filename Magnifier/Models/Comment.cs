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
        public Comment(int _commentId, ScratchComment _comment, bool _isReply, List<Comment> _replies)
        {
            commentId = _commentId;
            comment = _comment;
            isReply = _isReply;
            replies = _replies;

            reactions = new List<UserReaction>();
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public string _id { get; set; }

        public int commentId { get; set; }

        public ScratchComment comment { get; set; }

        public List<UserReaction> reactions { get; set; }

        public bool isPinned { get; set; }

        public bool isReply { get; set; }

        public List<Comment> replies { get; set; }
    }
}