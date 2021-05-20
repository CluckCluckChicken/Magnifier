using Magnifier.Models;
using Magnifier.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Magnifier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly CommentService commentService;
        private readonly ReactionService reactionService;

        public CommentsController(CommentService _commentService, ReactionService _reactionService)
        {
            commentService = _commentService;
            reactionService = _reactionService;
        }

        [HttpGet("{commentId}")]
        public ActionResult GetComment(int commentId)
        {
            if (commentService.Get(commentId) != null)
            {
                return Ok(commentService.Get(commentId));
            }

            return NotFound("comment has never been reacted to");
        }

        [HttpGet("projects/{projectId}/{page}")]
        public async System.Threading.Tasks.Task<ActionResult> GetProjectCommentsAsync(int projectId, int page)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync($"https://api.scratch.mit.edu/projects/{projectId}");
            var data = await response.Content.ReadAsStringAsync();

            ScratchProject project = JsonConvert.DeserializeObject<ScratchProject>(data);

            if (project.author == null)
            {
                return BadRequest("that project doesnt exist");
            }

            string projectOwner = project.author.username;

            response = await client.GetAsync($"https://api.scratch.mit.edu/users/{projectOwner}/projects/{projectId}/comments?offset={(page - 1) * 20}");
            data = await response.Content.ReadAsStringAsync();

            List<ScratchComment> comments = JsonConvert.DeserializeObject<List<ScratchComment>>(data);

            List<Comment> dbComments = commentService.Get();

            foreach (ScratchComment comment in comments)
            {
                if (dbComments.Find(dbComment => dbComment.commentId == comment.id) == null)
                {
                    commentService.Create(new Comment(comment.id, comment));
                }
            }

            List<Comment> matchingComments = commentService.Get().FindAll(comment => comments.Find(scratchComment => scratchComment.id == comment.commentId) != null);

            return Ok(System.Text.Json.JsonSerializer.Serialize(matchingComments));
        }

        [HttpPut("{projectId}/{commentId}/reactions")]
        [Authorize]
        public async Task<ActionResult> PutReactionAsync(int projectId, int commentId, string reaction)
        {
            Comment comment;

            string projectUrl = $"https://api.scratch.mit.edu/users/potatophant/projects/{projectId}";

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(projectUrl);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            if (commentService.Get(commentId) == null)
            {
                string commentUrl = $"https://api.scratch.mit.edu/users/potatophant/projects/{projectId}/comments/{commentId}";

                response = await client.GetAsync(commentUrl);

                comment = commentService.Create(new Comment(commentId, JsonConvert.DeserializeObject<ScratchComment>(await response.Content.ReadAsStringAsync())));
            }
            else
            {
                comment = commentService.Get(commentId);
            }

            if (comment.reactions == null)
            {
                comment.reactions = new List<UserReaction>();
            }

            if (reactionService.Get(reaction) != null)
            {
                UserReaction userReaction = new UserReaction(HttpContext.User.Claims.ToList().Find(claim => claim.Type == "username").Value, reaction);

                comment.reactions.Add(userReaction);

                commentService.Update(commentId, comment);

                return Accepted();
            }

            return NotFound("that reaction doesnt exist");
        }

        [HttpPut("{projectId}/{commentId}/pin")]
        [Authorize]
        public async Task<ActionResult> PinCommentAsync(int projectId, int commentId, bool pin = true)
        {
            Comment comment;

            if (commentService.Get(commentId) == null)
            {
                string commentUrl = $"https://api.scratch.mit.edu/users/potatophant/projects/{projectId}/comments/{commentId}";

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(commentUrl);

                comment = commentService.Create(new Comment(commentId, JsonConvert.DeserializeObject<ScratchComment>(await response.Content.ReadAsStringAsync())));
            }
            else
            {
                comment = commentService.Get(commentId);
            }

            comment.isPinned = pin;

            commentService.Update(commentId, comment);

            return Accepted();
        }

        [HttpDelete("{commentId}/reactions")]
        [Authorize]
        public ActionResult DeleteReaction(int commentId, string reaction)
        {
            string username = HttpContext.User.Claims.ToList().Find(claim => claim.Type == "username").Value;

            Comment comment = commentService.Get(commentId);

            if (comment != null)
            {
                UserReaction userReaction = comment.reactions.Find(userReaction => userReaction.reaction == reaction && userReaction.user == username);
                if (userReaction != null)
                {
                    comment.reactions.Remove(userReaction);

                    commentService.Update(comment.commentId, comment);

                    return Accepted();
                }

                return NotFound("comment doesnt have this reaction");
            }

            return NotFound("comment has never been reacted to");
        }
    }
}