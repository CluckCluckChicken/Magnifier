using HtmlAgilityPack;
using Magnifier.Models;
using Magnifier.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Magnifier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly CommentService commentService;
        private readonly ReactionService reactionService;
        private readonly UserService userService;
        private readonly HttpClient client;

        public CommentsController(CommentService _commentService, ReactionService _reactionService, UserService _userService)
        {
            commentService = _commentService;
            reactionService = _reactionService;
            userService = _userService;

            client = new HttpClient();
        }

        private async Task<ScratchRequestResponse> GetScratchProject(int projectId)
        {
            HttpResponseMessage response = await client.GetAsync($"https://api.scratch.mit.edu/projects/{projectId}");
            var data = await response.Content.ReadAsStringAsync();

            return new ScratchRequestResponse(response, JsonConvert.DeserializeObject<ScratchProject>(data));
        }

        private async Task<ScratchRequestResponse> GetScratchComment(int projectId, int commentId)
        {
            ScratchRequestResponse requestResponse = await GetScratchProject(projectId);

            if (!requestResponse.succeeded)
            {
                return new ScratchRequestResponse(requestResponse.response);
            }

            ScratchProject project = requestResponse.project;

            string projectOwner = project.author.username;

            HttpResponseMessage response = await client.GetAsync($"https://api.scratch.mit.edu/users/{projectOwner}/projects/{projectId}/comments/{commentId}");
            var data = await response.Content.ReadAsStringAsync();

            return new ScratchRequestResponse(requestResponse.response, _comment: JsonConvert.DeserializeObject<ScratchComment>(data));
        }

        /*private async Task<ScratchRequestResponse> GetScratchCommentReplies(int projectId, int commentId)
        {
            ScratchRequestResponse requestResponse = await GetScratchProject(projectId);

            if (!requestResponse.succeeded)
            {
                return new ScratchRequestResponse(requestResponse.response);
            }

            ScratchProject project = requestResponse.project;

            string projectOwner = project.author.username;

            requestResponse = await GetScratchComment(projectId, commentId);

            if (!requestResponse.succeeded)
            {
                return new ScratchRequestResponse(requestResponse.response);
            }

            ScratchComment comment = requestResponse.comment;

            HttpResponseMessage response = await client.GetAsync($"https://api.scratch.mit.edu/users/{projectOwner}/projects/{projectId}/comments/{comment.id}/replies");
            var data = await response.Content.ReadAsStringAsync();

            return new ScratchRequestResponse(requestResponse.response, _comments: JsonConvert.DeserializeObject<List<ScratchComment>>(data));
        }*/

        private async Task<ScratchRequestResponse> GetScratchCommentReplies(string projectOwner, int projectId, int commentId)
        {
            HttpResponseMessage response = await client.GetAsync($"https://api.scratch.mit.edu/users/{projectOwner}/projects/{projectId}/comments/{commentId}/replies");

            if (!response.IsSuccessStatusCode)
            {
                return new ScratchRequestResponse(response);
            }

            var data = await response.Content.ReadAsStringAsync();

            List<int> replies = new List<int>();

            foreach (ScratchComment comment in JsonConvert.DeserializeObject<List<ScratchComment>>(data))
            {
                replies.Add(comment.id);
            }

            return new ScratchRequestResponse(response, _comments: replies);
        }

        [HttpGet("{commentId}")]
        public ActionResult GetComment(int commentId)
        {
            if (commentService.Get(commentId) == null)
            {
                return NotFound();
            }

            return Ok(commentService.Get(commentId));
        }

        /*[HttpGet("{projectId}/{commentId}")]
        public async Task<ActionResult> GetCommentAsync(int projectId, int commentId)
        {
            if (commentService.Get(commentId) == null)
            {
                ScratchRequestResponse requestResponse = await GetScratchComment(projectId, commentId);

                if (!requestResponse.succeeded)
                {
                    return NotFound(requestResponse.statusCode.ToString());
                }

                ScratchComment comment = requestResponse.comment;

                requestResponse = await GetScratchCommentReplies(projectId, comment.id);

                if (!requestResponse.succeeded)
                {
                    return NotFound(requestResponse.statusCode.ToString());
                }

                var scratchCommentReplies = requestResponse.comments;

                List<int> replies = new List<int>();

                foreach (ScratchComment scratchComment in scratchCommentReplies)
                {
                    commentService.Create(new Comment(scratchComment.id, scratchComment, true, new List<int>()));
                    replies.Add(scratchComment.id);
                }

                commentService.Create(new Comment(comment.id, comment, false, replies));
            }

            return Ok(commentService.Get(commentId));
        }*/

        [HttpGet("projects/{projectId}/exists")]
        public async Task<ActionResult> GetIfProjectExistsAsync(int projectId)
        {
            ScratchRequestResponse requestResponse = await GetScratchProject(projectId);

            if (!requestResponse.succeeded)
            {
                return NotFound(requestResponse.statusCode.ToString());
            }

            return Ok();
        }

        [HttpGet("users/{username}/exists")]
        public async Task<ActionResult> GetIfUserExistsAsync(string username)
        {
            HttpResponseMessage response = await client.GetAsync($"https://scratch.mit.edu/users/{username}/");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound(response.StatusCode.ToString());
            }

            return Ok();
        }

        [HttpGet("studios/{studioId}/exists")]
        public async Task<ActionResult> GetIfStudioExistsAsync(int studioId)
        {
            HttpResponseMessage response = await client.GetAsync($"https://scratch.mit.edu/studios/{studioId}/");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound(response.StatusCode.ToString());
            }

            return Ok();
        }

        /*[HttpGet("projects/{projectId}/{page}")]
        public async Task<ActionResult> GetProjectCommentsAsync(int projectId, int page)
        {
            ScratchRequestResponse requestResponse = await GetScratchProject(projectId);

            if (!requestResponse.succeeded)
            {
                return NotFound(requestResponse.statusCode.ToString());
            }

            ScratchProject project = requestResponse.project;

            if (project.author == null)
            {
                return BadRequest("that project doesnt exist");
            }

            string projectOwner = project.author.username;

            var response = await client.GetAsync($"https://api.scratch.mit.edu/users/{projectOwner}/projects/{projectId}/comments?offset={(page - 1) * 20}");
            var data = await response.Content.ReadAsStringAsync();

            List<ScratchComment> comments = JsonConvert.DeserializeObject<List<ScratchComment>>(data);

            List<Comment> dbComments = commentService.Get();

            foreach (ScratchComment comment in comments)
            {
                requestResponse = await GetScratchCommentReplies(projectOwner, projectId, comment.id);

                if (!requestResponse.succeeded)
                {
                    return NotFound(requestResponse.statusCode.ToString());
                }

                var scratchCommentReplies = requestResponse.comments;

                List<int> replies = new List<int>();

                foreach (int scratchComment in scratchCommentReplies)
                {
                    if (commentService.Get(scratchComment) == null)
                    {
                        ScratchRequestResponse imrunningoutofvariablenames = await GetScratchComment(projectId, scratchComment);

                        commentService.Create(new Comment(scratchComment, imrunningoutofvariablenames.comment, true, new List<int>()));
                    }
                    replies.Add(scratchComment);
                }

                if (dbComments.Find(dbComment => dbComment.commentId == comment.id) == null)
                {
                    commentService.Create(new Comment(comment.id, comment, false, replies));
                }
                else
                {
                    Comment sendhelp = commentService.Get(comment.id);

                    sendhelp.replies = replies;

                    commentService.Update(comment.id, sendhelp);
                }
            }

            dbComments = commentService.Get();

            List<Comment> matchingComments = commentService.Get().FindAll(comment => comments.Find(scratchComment => scratchComment.id == comment.commentId) != null);

            matchingComments = matchingComments
                .Where(p => p.comment.datetime_created.HasValue)
                .OrderBy(p => p.comment.datetime_created.Value)
                .Reverse()
                .ToList();

            return Ok(System.Text.Json.JsonSerializer.Serialize(matchingComments));
        }*/

        [HttpGet("projects/{projectId}/{page}")]
        public async Task<ActionResult> GetProjectCommentsAsync(int projectId, int page)
        {
            ScratchRequestResponse requestResponse = await GetScratchProject(projectId);

            if (!requestResponse.succeeded)
            {
                return NotFound(requestResponse.statusCode.ToString());
            }

            ScratchProject project = requestResponse.project;

            if (project.author == null)
            {
                return BadRequest("that project doesnt exist");
            }

            string projectOwner = project.author.username;

            var response = await client.GetAsync($"https://api.scratch.mit.edu/users/{projectOwner}/projects/{projectId}/comments?offset={(page - 1) * 20}");
            var data = await response.Content.ReadAsStringAsync();

            List<ScratchComment> comments = JsonConvert.DeserializeObject<List<ScratchComment>>(data);

            List<Comment> dbComments = commentService.Get();

            foreach (ScratchComment comment in comments)
            {
                requestResponse = await GetScratchCommentReplies(projectOwner, projectId, comment.id);

                if (!requestResponse.succeeded)
                {
                    return NotFound(requestResponse.statusCode.ToString());
                }

                var scratchCommentReplies = requestResponse.comments;

                List<int> replies = new List<int>();

                foreach (int scratchComment in scratchCommentReplies)
                {
                    if (dbComments.Find(comment => comment.commentId == scratchComment) == null)
                    {
                        ScratchRequestResponse imrunningoutofvariablenames = await GetScratchComment(projectId, scratchComment);

                        new Thread(() =>
                        {
                            commentService.Create(new Comment(scratchComment, imrunningoutofvariablenames.comment, true, new List<int>()));
                        }).Start();
                    }
                    replies.Add(scratchComment);
                }

                if (dbComments.Find(dbComment => dbComment.commentId == comment.id) == null)
                {
                    commentService.Create(new Comment(comment.id, comment, false, replies));
                }
                else
                {
                    Comment sendhelp = dbComments.Find(dbComment => dbComment.commentId == comment.id);

                    sendhelp.replies = replies;

                    commentService.Update(comment.id, sendhelp);
                }
            }

            dbComments = commentService.Get();

            List<Comment> matchingComments = commentService.Get().FindAll(comment => comments.Find(scratchComment => scratchComment.id == comment.commentId) != null);

            matchingComments = matchingComments
                .Where(p => p.comment.datetime_created.HasValue)
                .OrderBy(p => p.comment.datetime_created.Value)
                .Reverse()
                .ToList();

            return Ok(System.Text.Json.JsonSerializer.Serialize(matchingComments));
        }

        /*[HttpGet("projects/{projectId}/{page}")]
        public async Task<ActionResult> GetProjectCommentsAsync(int projectId, int page)
        {
            ScratchRequestResponse requestResponse = await GetScratchProject(projectId);

            if (!requestResponse.succeeded)
            {
                return NotFound(requestResponse.statusCode.ToString());
            }

            ScratchProject project = requestResponse.project;

            if (project.author == null)
            {
                return BadRequest("that project doesnt exist");
            }

            string projectOwner = project.author.username;

            var response = await client.GetAsync($"https://api.scratch.mit.edu/users/{projectOwner}/projects/{projectId}/comments?offset={(page - 1) * 20}");
            var data = await response.Content.ReadAsStringAsync();

            List<ScratchComment> scratchComments = JsonConvert.DeserializeObject<List<ScratchComment>>(data);

            List<Comment> dbComments = commentService.Get();

            List<Comment> comments = new List<Comment>();

            foreach (ScratchComment scratchComment in scratchComments)
            {
                requestResponse = await GetScratchCommentReplies(projectOwner, projectId, scratchComment.id);

                if (!requestResponse.succeeded)
                {
                    return NotFound(requestResponse.statusCode.ToString());
                }

                var scratchCommentReplies = requestResponse.comments;

                List<int> replies = new List<int>();

                foreach (int scratchCommentReply in scratchCommentReplies)
                {
                    ScratchRequestResponse imrunningoutofvariablenames = await GetScratchComment(projectId, scratchCommentReply);
                    replies.Add(scratchCommentReply);
                }

                if (dbComments.Find(comment => comment.commentId == scratchComment.id) == null)
                {
                    comments.Add(new Comment(scratchComment.id, scratchComment, false, replies));
                }
                else
                {
                    comments.Add(dbComments.Find(comment => comment.commentId == scratchComment.id));
                }
            }

            return Ok(System.Text.Json.JsonSerializer.Serialize(comments));
        }*/

        [HttpGet("users/{username}/{page}")]
        public async Task<ActionResult> GetUserCommentsAsync(string username, int page)
        {
            string response;

            try
            {
                response = await client.GetStringAsync($"https://scratch.mit.edu/site-api/comments/user/{username}?page={page}");
            }
            catch
            {
                return NotFound();
            }

            HtmlDocument html = new HtmlDocument();

            html.LoadHtml(response);

            HtmlNodeCollection commentNodes = html.DocumentNode.SelectNodes("//div[@class=\"comment \"]");

            List<Comment> comments = new List<Comment>();

            foreach (HtmlNode node in commentNodes)
            {
                HtmlNode info = node.SelectSingleNode(".//div[@class=\"info\"]");
                HtmlNode user = node.SelectSingleNode(".//a[@id=\"comment-user\"]");
                ScratchCommentAuthor author = new ScratchCommentAuthor(info.SelectSingleNode(".//div[@class=\"name\"]").InnerText.Trim(), user.SelectSingleNode(".//img[@class=\"avatar\"]").Attributes["src"].Value);
                ScratchComment scratchComment = new ScratchComment(int.Parse(node.Attributes["data-comment-id"].Value), info.SelectSingleNode(".//div[@class=\"content\"]").InnerText.Trim().Replace("\n      ", ""), author, DateTime.Parse(info.SelectSingleNode(".//span[@class=\"time\"]").Attributes["title"].Value));

                List<int> replies = new List<int>();

                if (!node.ParentNode.HasClass("reply"))
                {
                    foreach (HtmlNode replyContainer in node.ParentNode.SelectSingleNode(".//ul[@class=\"replies\"]").ChildNodes)
                    {
                        if (replyContainer.SelectSingleNode(".//div[@class=\"comment \"]") != null)
                        {
                            replies.Add(int.Parse(replyContainer.SelectSingleNode(".//div[@class=\"comment \"]").Attributes["data-comment-id"].Value));
                        }
                    }
                }

                comments.Add(new Comment(scratchComment.id, scratchComment, node.ParentNode.HasClass("reply"), replies));
            }

            List<Comment> dbComments = commentService.Get();

            foreach (Comment comment in comments)
            {
                if (dbComments.Find(dbComment => dbComment.commentId == comment.commentId) == null)
                {
                    commentService.Create(comment);
                }
            }

            dbComments = commentService.Get();

            List<Comment> matchingComments = commentService.Get().FindAll(comment => comments.Find(comment2 => comment2.commentId == comment.commentId) != null);

            matchingComments = matchingComments
                .Where(p => p.comment.datetime_created.HasValue)
                .OrderBy(p => p.comment.datetime_created.Value)
                .Reverse()
                .ToList();

            return Ok(System.Text.Json.JsonSerializer.Serialize(matchingComments));
        }

        [HttpGet("studios/{studioId}/{page}")]
        public async Task<ActionResult> GetUserCommentsAsync(int studioId, int page)
        {
            string response;

            try
            {
                response = await client.GetStringAsync($"https://scratch.mit.edu/site-api/comments/gallery/{studioId}?page={page}");
            }
            catch
            {
                return NotFound();
            }

            HtmlDocument html = new HtmlDocument();

            html.LoadHtml(response);

            HtmlNodeCollection commentNodes = html.DocumentNode.SelectNodes("//div[@class=\"comment \"]");

            List<Comment> comments = new List<Comment>();

            foreach (HtmlNode node in commentNodes)
            {
                HtmlNode info = node.SelectSingleNode(".//div[@class=\"info\"]");
                HtmlNode user = node.SelectSingleNode(".//a[@id=\"comment-user\"]");
                ScratchCommentAuthor author = new ScratchCommentAuthor(info.SelectSingleNode(".//div[@class=\"name\"]").InnerText.Trim(), user.SelectSingleNode(".//img[@class=\"avatar\"]").Attributes["src"].Value);
                ScratchComment scratchComment = new ScratchComment(int.Parse(node.Attributes["data-comment-id"].Value), info.SelectSingleNode(".//div[@class=\"content\"]").InnerText.Trim().Replace("\n      ", ""), author, DateTime.Parse(info.SelectSingleNode(".//span[@class=\"time\"]").Attributes["title"].Value));

                List<int> replies = new List<int>();

                if (!node.ParentNode.HasClass("reply"))
                {
                    foreach (HtmlNode replyContainer in node.ParentNode.SelectSingleNode(".//ul[@class=\"replies\"]").ChildNodes)
                    {
                        if (replyContainer.SelectSingleNode(".//div[@class=\"comment \"]") != null)
                        {
                            replies.Add(int.Parse(replyContainer.SelectSingleNode(".//div[@class=\"comment \"]").Attributes["data-comment-id"].Value));
                        }
                    }
                }

                comments.Add(new Comment(scratchComment.id, scratchComment, node.ParentNode.HasClass("reply"), replies));
            }

            List<Comment> dbComments = commentService.Get();

            foreach (Comment comment in comments)
            {
                if (dbComments.Find(dbComment => dbComment.commentId == comment.commentId) == null)
                {
                    commentService.Create(comment);
                }
            }

            dbComments = commentService.Get();

            List<Comment> matchingComments = commentService.Get().FindAll(comment => comments.Find(comment2 => comment2.commentId == comment.commentId) != null);

            matchingComments = matchingComments
                .Where(p => p.comment.datetime_created.HasValue)
                .OrderBy(p => p.comment.datetime_created.Value)
                .Reverse()
                .ToList();

            return Ok(System.Text.Json.JsonSerializer.Serialize(matchingComments));
        }

        /*[HttpPut("{projectId}/{commentId}/reactions")]
        [Authorize]
        public async Task<ActionResult> PutReactionAsync(int projectId, int commentId, string reaction)
        {
            ScratchRequestResponse requestResponse = await GetScratchProject(projectId);

            if (!requestResponse.succeeded)
            {
                return NotFound(requestResponse.statusCode.ToString());
            }

            ScratchProject project = requestResponse.project;

            if (project.author == null)
            {
                return BadRequest("that project doesnt exist");
            }

            string projectOwner = project.author.username;

            Comment comment;

            string projectUrl = $"https://api.scratch.mit.edu/users/{projectOwner}/projects/{projectId}";

            var response = await client.GetAsync(projectUrl);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            if (commentService.Get(commentId) == null)
            {
                string commentUrl = $"https://api.scratch.mit.edu/users/{projectOwner}/projects/{projectId}/comments/{commentId}";

                response = await client.GetAsync(commentUrl);

                var repliesResponse = await client.GetAsync($"https://api.scratch.mit.edu/users/{projectOwner}/projects/{projectId}/comments/{commentId}/replies");
                var data = await repliesResponse.Content.ReadAsStringAsync();

                var scratchCommentReplies = JsonConvert.DeserializeObject<List<ScratchComment>>(data);

                List<int> replies = new List<int>();

                foreach (ScratchComment scratchComment in scratchCommentReplies)
                {
                    commentService.Create(new Comment(scratchComment.id, scratchComment, true, new List<int>()));
                    replies.Add(scratchComment.id);
                }

                comment = commentService.Create(new Comment(commentId, JsonConvert.DeserializeObject<ScratchComment>(await response.Content.ReadAsStringAsync()), false, replies));
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
                string username = HttpContext.User.Claims.ToList().Find(claim => claim.Type == "username").Value;

                UserReaction userReaction = new UserReaction(username, reaction);

                if (comment.reactions.Find(userReaction => userReaction.user == username && userReaction.reaction == reaction) == null)
                {
                    comment.reactions.Add(userReaction);
                }
                else
                {
                    comment.reactions.Remove(comment.reactions.Find(userReaction => userReaction.user == username && userReaction.reaction == reaction));
                }

                commentService.Update(commentId, comment);

                return Accepted();
            }

            return NotFound("that reaction doesnt exist");
        }*/

        [HttpPut("{commentId}/reactions")]
        [Authorize]
        public ActionResult PutReaction(int commentId, string reaction)
        {
            User user = userService.Get(HttpContext.User.Claims.ToList().Find(claim => claim.Type == "username").Value);

            if (user.isBanned)
            {
                return Forbid();
            }

            Comment comment;

            if (commentService.Get(commentId) == null)
            {
                return NotFound("that comment doesnt exist");
            }

            comment = commentService.Get(commentId);

            if (comment.reactions == null)
            {
                comment.reactions = new List<UserReaction>();
            }

            if (reactionService.Get(reaction) != null)
            {
                string username = HttpContext.User.Claims.ToList().Find(claim => claim.Type == "username").Value;

                UserReaction userReaction = new UserReaction(username, reaction);

                if (comment.reactions.Find(userReaction => userReaction.user == username && userReaction.reaction == reaction) == null)
                {
                    comment.reactions.Add(userReaction);
                }
                else
                {
                    comment.reactions.Remove(comment.reactions.Find(userReaction => userReaction.user == username && userReaction.reaction == reaction));
                }

                commentService.Update(commentId, comment);

                return Accepted();
            }

            return NotFound("that reaction doesnt exist");
        }

        [HttpPut("{projectId}/{commentId}/pin")]
        [Authorize]
        public async Task<ActionResult> PinCommentAsync(int projectId, int commentId, bool pin = true)
        {
            ScratchRequestResponse requestResponse = await GetScratchProject(projectId);

            if (!requestResponse.succeeded)
            {
                return NotFound(requestResponse.statusCode.ToString());
            }

            ScratchProject project = requestResponse.project;

            if (project.author == null)
            {
                return BadRequest("that project doesnt exist");
            }

            string projectOwner = project.author.username;

            Comment comment;

            if (commentService.Get(commentId) == null)
            {
                string commentUrl = $"https://api.scratch.mit.edu/users/{projectOwner}/projects/{projectId}/comments/{commentId}";

                var client = new HttpClient();
                var response = await client.GetAsync(commentUrl);

                var repliesResponse = await client.GetAsync($"https://api.scratch.mit.edu/users/{projectOwner}/projects/{projectId}/comments/{commentId}/replies");
                var data  = await repliesResponse.Content.ReadAsStringAsync();

                var scratchCommentReplies = JsonConvert.DeserializeObject<List<ScratchComment>>(data);

                List<int> replies = new List<int>();

                foreach (ScratchComment scratchComment in scratchCommentReplies)
                {
                    commentService.Create(new Comment(scratchComment.id, scratchComment, true, new List<int>()));
                    replies.Add(scratchComment.id);
                }

                comment = commentService.Create(new Comment(commentId, JsonConvert.DeserializeObject<ScratchComment>(await response.Content.ReadAsStringAsync()), false, replies));
            }
            else
            {
                comment = commentService.Get(commentId);
            }

            comment.isPinned = pin;

            commentService.Update(commentId, comment);

            return Accepted();
        }

    }
}