using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magnifier.Models
{
    public record Comment
    {
        public Comment(int commentId, ScratchComment comment, List<int> replies)
        {
            this.commentId = commentId;
            this.comment = comment;
            this.replies = replies;
        }

        public string _id { get; set; }

        public int commentId { get; set; }

        public ScratchComment comment { get; set; }

        public List<UserReaction> reactions { get; set; }

        public bool isPinned { get; set; }

        public List<int> replies { get; set; }
    }
}