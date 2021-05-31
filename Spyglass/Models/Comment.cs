using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magnifier.Models
{
    public record Comment
    {
        public Comment(int commentId, ScratchComment comment, bool isReply, List<Magnifier.Models.Comment> replies)
        {
            this.commentId = commentId;
            this.comment = comment;
            this.isReply = isReply;
            this.replies = replies;

            reactions = new List<UserReaction>();
        }

        public string _id { get; set; }

        public int commentId { get; set; }

        public ScratchComment comment { get; set; }

        public List<UserReaction> reactions { get; set; }

        public bool isPinned { get; set; }

        public bool isReply { get; set; }

        public List<Magnifier.Models.Comment> replies { get; set; }
    }
}