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

        public Comment(int commentId, ScratchComment comment, Residence residence, string residenceId, bool isReply, List<Comment> replies)
        {
            this.commentId = commentId;
            this.comment = comment;
            this.residence = residence;
            this.residenceId = residenceId;
            this.isReply = isReply;
            this.replies = replies;

            reactions = new List<UserReaction>();
        }

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
