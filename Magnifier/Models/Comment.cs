using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magnifier.Models
{
    public enum Residence
    {
        Project,
        User,
        Studio
    }

    public record Comment
    {
        public Comment() { }

        public Comment(int _commentId, ScratchComment _comment, Residence _residence, string _residenceId, bool _isReply, List<Comment> _replies)
        {
            commentId = _commentId;
            comment = _comment;
            residence = _residence;
            residenceId = _residenceId;
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

        public Residence residence { get; set; }

        public string residenceId { get; set; }

        public List<UserReaction> reactions { get; set; }

        public bool isPinned { get; set; }

        public bool isReply { get; set; }

        public List<Comment> replies { get; set; }
    }
}
