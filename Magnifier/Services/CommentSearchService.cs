using Magnifier.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Magnifier.Services
{
    public class CommentSearchService
    {
        private readonly CommentService commentService;

        public CommentSearchService(CommentService _commentService)
        {
            commentService = _commentService;
        }

        public List<Comment> QueryComments(List<CommentSearchRequirement> query)
        {
            List<Comment> comments = commentService.Get();

            foreach (CommentSearchRequirement requirement in query)
            {
                switch (requirement.prefix)
                {
                    case '+':
                        comments.RemoveAll(comment => !GetBinding(requirement.requirement, comment).Contains(requirement.value));
                        break;
                    case '-':
                        comments.RemoveAll(comment => GetBinding(requirement.requirement, comment).Contains(requirement.value));
                        break;
                    case '=':
                        comments.RemoveAll(comment => GetBinding(requirement.requirement, comment) != requirement.value);
                        break;
                }
            }

            return comments;
        }

        public string GetBinding(Requirement requirement, Comment comment)
        {
            string[] bindings =
            {
                typeof(Comment).GetProperty("commentId").GetValue(comment).ToString(),
                typeof(ScratchComment).GetProperty("content").GetValue(comment.comment).ToString(),
                typeof(ScratchCommentAuthor).GetProperty("username").GetValue(comment.comment.author).ToString()
            };

            return bindings[(int)requirement];
        }
    }
}
