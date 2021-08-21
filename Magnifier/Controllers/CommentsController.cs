using HtmlAgilityPack;
using Magnifier.Models;
using Magnifier.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        private readonly CommentSearchService commentSearchService;
        private readonly HttpClient client;

        public CommentsController(CommentService _commentService, ReactionService _reactionService, UserService _userService, CommentSearchService _commentSearchService)
        {
            commentService = _commentService;
            reactionService = _reactionService;
            userService = _userService;
            commentSearchService = _commentSearchService;

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

        private async Task<ScratchRequestResponse> GetScratchCommentReplies(string projectOwner, int projectId, int commentId)
        {
            HttpResponseMessage response = await client.GetAsync($"https://api.scratch.mit.edu/users/{projectOwner}/projects/{projectId}/comments/{commentId}/replies");

            if (!response.IsSuccessStatusCode)
            {
                return new ScratchRequestResponse(response);
            }

            var data = await response.Content.ReadAsStringAsync();

            List<Comment> replies = new List<Comment>();

            foreach (ScratchComment comment in JsonConvert.DeserializeObject<List<ScratchComment>>(data))
            {
                Comment c = commentService.Get(comment.id);

                if (c == null)
                {
                    c = commentService.Create(new Comment(comment.id, comment, Residence.Project, projectId.ToString(), true, new List<Comment>()));
                }

                replies.Add(c);
            }

            return new ScratchRequestResponse(response, _comments: replies);
        }

        /// <summary>
        /// Get a comment from its id
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        [HttpGet("{commentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult GetComment(int commentId)
        {
            Comment comment = commentService.Get(commentId);

            if (comment == null)
            {
                return NotFound();
            }

            return Ok(comment);
        }

        /// <summary>
        /// Get a comment's reactions
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        [HttpGet("{commentId}/reactions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult GetCommentReactions(int commentId)
        {
            Comment comment = commentService.Get(commentId);

            if (comment == null)
            {
                return Ok(new List<UserReaction>());
            }

            return Ok(comment.reactions);
        }

        /// <summary>
        /// Check if a Scratch project exists
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet("projects/{projectId}/exists")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetIfProjectExistsAsync(int projectId)
        {
            ScratchRequestResponse requestResponse = await GetScratchProject(projectId);

            if (!requestResponse.succeeded)
            {
                return NotFound(requestResponse.statusCode.ToString());
            }

            return Ok();
        }

        /// <summary>
        /// Check if a Scratch user exists
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpGet("users/{username}/exists")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetIfUserExistsAsync(string username)
        {
            HttpResponseMessage response = await client.GetAsync($"https://scratch.mit.edu/users/{username}/");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound(response.StatusCode.ToString());
            }

            return Ok();
        }

        /// <summary>
        /// Check if a Scratch studio exists
        /// </summary>
        /// <param name="studioId"></param>
        /// <returns></returns>
        [HttpGet("studios/{studioId}/exists")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetIfStudioExistsAsync(int studioId)
        {
            HttpResponseMessage response = await client.GetAsync($"https://scratch.mit.edu/studios/{studioId}/");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound(response.StatusCode.ToString());
            }

            return Ok();
        }

        /// <summary>
        /// Get the comments on a project
        /// </summary>
        /// <param name="projectId"></param>
        ///<param name="page"></param>
        /// <returns></returns>
        [HttpGet("projects/{projectId}/{page}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

            List<ScratchComment> responseComments = JsonConvert.DeserializeObject<List<ScratchComment>>(data);

            List<Comment> comments = new List<Comment>();

            foreach (ScratchComment comment in responseComments)
            {
                requestResponse = await GetScratchCommentReplies(projectOwner, projectId, comment.id);

                if (!requestResponse.succeeded)
                {
                    return NotFound(requestResponse.statusCode.ToString());
                }

                List<Comment> replies = requestResponse.comments;

                Comment c = new Comment(comment.id, comment, Residence.Project, projectId.ToString(), false, replies);

                comments.Add(c);
            }

            List<int> commentIds = comments.Select(comment => comment.commentId).ToList();

            List<Comment> dbComments = commentService.GetMany(commentIds);

            List<Comment> workingComments = new List<Comment>();

            foreach (Comment comment in comments)
            {
                workingComments.Add(new Comment
                {
                    _id = comment._id,
                    commentId = comment.commentId,
                    comment = comment.comment,
                    residence = comment.residence,
                    residenceId = comment.residenceId,
                    reactions = comment.reactions,
                    isPinned = comment.isPinned,
                    isReply = comment.isReply,
                    replies = comment.replies
                });
            }

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(() =>
            {
                foreach (Comment comment in workingComments)
                {
                    if (dbComments.Find(dbComment => dbComment.commentId == comment.commentId) == null)
                    {
                        commentService.Create(comment);
                    }
                    else
                    {
                        Comment c = dbComments.Find(dbComment => dbComment.commentId == comment.commentId);
                        comment.reactions = c.reactions;
                        commentService.Update(comment.commentId, comment);
                    }
                }
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            foreach (Comment dbComment in dbComments)
            {
                Comment c = dbComment;
                c.replies = comments.Find(comment => comment.commentId == dbComment.commentId).replies;
                comments[comments.IndexOf(comments.Find(comment => comment.commentId == dbComment.commentId))] = c;
            }

            return Ok(JsonConvert.SerializeObject(comments));
        }

        /// <summary>
        /// Get the comments on a user profile
        /// </summary>
        /// <param name="username"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet("users/{username}/{page}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

            if (commentNodes == null)
            {
                return Ok(JsonConvert.SerializeObject(new List<Comment>()));
            }

            foreach (HtmlNode node in commentNodes)
            {
                HtmlNode info = node.SelectSingleNode(".//div[@class=\"info\"]");
                HtmlNode user = node.SelectSingleNode(".//a[@id=\"comment-user\"]");
                ScratchCommentAuthor author = new ScratchCommentAuthor(info.SelectSingleNode(".//div[@class=\"name\"]").InnerText.Trim(), user.SelectSingleNode(".//img[@class=\"avatar\"]").Attributes["src"].Value);
                ScratchComment scratchComment = new ScratchComment(int.Parse(node.Attributes["data-comment-id"].Value), info.SelectSingleNode(".//div[@class=\"content\"]").InnerText.Trim().Replace("\n      ", ""), author, DateTime.Parse(info.SelectSingleNode(".//span[@class=\"time\"]").Attributes["title"].Value));

                List<Comment> replies = new List<Comment>();

                if (!node.ParentNode.HasClass("reply"))
                {
                    foreach (HtmlNode replyContainer in node.ParentNode.SelectSingleNode(".//ul[@class=\"replies\"]").ChildNodes)
                    {
                        if (replyContainer.SelectSingleNode(".//div[@class=\"comment \"]") != null)
                        {
                            HtmlNode replyContainerInfo = replyContainer.SelectSingleNode(".//div[@class=\"info\"]");
                            HtmlNode replyContainerUser = replyContainer.SelectSingleNode(".//a[@id=\"comment-user\"]");
                            ScratchCommentAuthor replyContainerUserAuthor = new ScratchCommentAuthor(replyContainerInfo.SelectSingleNode(".//div[@class=\"name\"]").InnerText.Trim(), replyContainerUser.SelectSingleNode(".//img[@class=\"avatar\"]").Attributes["src"].Value);
                            ScratchComment replyContainerUserScratchComment /* :) */ = new ScratchComment(int.Parse(replyContainer.SelectSingleNode(".//div[@class=\"comment \"]").Attributes["data-comment-id"].Value), replyContainerInfo.SelectSingleNode(".//div[@class=\"content\"]").InnerText.Trim().Replace("\n      ", ""), replyContainerUserAuthor, DateTime.Parse(replyContainerInfo.SelectSingleNode(".//span[@class=\"time\"]").Attributes["title"].Value));

                            Comment r = new Comment(replyContainerUserScratchComment.id, replyContainerUserScratchComment, Residence.User, username, true, new List<Comment>());

                            replies.Add(r);

                            comments.Add(r);
                        }
                    }

                    Comment c = new Comment(scratchComment.id, scratchComment, Residence.User, username, false, replies);

                    comments.Add(c);
                }
            }

            List<int> commentIds = comments.Select(comment => comment.commentId).ToList();

            List<Comment> dbComments = commentService.GetMany(commentIds);

            List<Comment> workingComments = new List<Comment>();

            foreach (Comment comment in comments)
            {
                workingComments.Add(new Comment
                {
                    _id = comment._id,
                    commentId = comment.commentId,
                    comment = comment.comment,
                    residence = comment.residence,
                    residenceId = comment.residenceId,
                    reactions = comment.reactions,
                    isPinned = comment.isPinned,
                    isReply = comment.isReply,
                    replies = comment.replies
                });
            }

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(() =>
            {
                foreach (Comment comment in workingComments)
                {
                    if (dbComments.Find(dbComment => dbComment.commentId == comment.commentId) == null)
                    {
                        commentService.Create(comment);
                    }
                    else
                    {
                        Comment c = dbComments.Find(dbComment => dbComment.commentId == comment.commentId);
                        c.residence = comment.residence;
                        c.residenceId = comment.residenceId;
                        c.replies = comment.replies;
                        commentService.Update(comment.commentId, c);
                    }
                }
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            foreach (Comment dbComment in dbComments)
            {
                Comment c = dbComment;
                c.replies = comments.Find(comment => comment.commentId == dbComment.commentId).replies;
                comments[comments.IndexOf(comments.Find(comment => comment.commentId == dbComment.commentId))] = c;
            }

            return Ok(JsonConvert.SerializeObject(comments));
        }

        /// <summary>
        /// Get the comments on a studio
        /// </summary>
        /// <param name="studioId"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet("studios/{studioId}/{page}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetStudioCommentsAsync(int studioId, int page)
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

            if (commentNodes == null)
            {
                return Ok(JsonConvert.SerializeObject(new List<Comment>()));
            }

            foreach (HtmlNode node in commentNodes)
            {
                HtmlNode info = node.SelectSingleNode(".//div[@class=\"info\"]");
                HtmlNode user = node.SelectSingleNode(".//a[@id=\"comment-user\"]");
                ScratchCommentAuthor author = new ScratchCommentAuthor(info.SelectSingleNode(".//div[@class=\"name\"]").InnerText.Trim(), user.SelectSingleNode(".//img[@class=\"avatar\"]").Attributes["src"].Value);
                ScratchComment scratchComment = new ScratchComment(int.Parse(node.Attributes["data-comment-id"].Value), info.SelectSingleNode(".//div[@class=\"content\"]").InnerText.Trim().Replace("\n      ", ""), author, DateTime.Parse(info.SelectSingleNode(".//span[@class=\"time\"]").Attributes["title"].Value));

                List<Comment> replies = new List<Comment>();

                if (!node.ParentNode.HasClass("reply"))
                {
                    foreach (HtmlNode replyContainer in node.ParentNode.SelectSingleNode(".//ul[@class=\"replies\"]").ChildNodes)
                    {
                        if (replyContainer.SelectSingleNode(".//div[@class=\"comment \"]") != null)
                        {
                            HtmlNode replyContainerInfo = replyContainer.SelectSingleNode(".//div[@class=\"info\"]");
                            HtmlNode replyContainerUser = replyContainer.SelectSingleNode(".//a[@id=\"comment-user\"]");
                            ScratchCommentAuthor replyContainerUserAuthor = new ScratchCommentAuthor(replyContainerInfo.SelectSingleNode(".//div[@class=\"name\"]").InnerText.Trim(), replyContainerUser.SelectSingleNode(".//img[@class=\"avatar\"]").Attributes["src"].Value);
                            ScratchComment replyContainerUserScratchComment /* :) */ = new ScratchComment(int.Parse(replyContainer.SelectSingleNode(".//div[@class=\"comment \"]").Attributes["data-comment-id"].Value), replyContainerInfo.SelectSingleNode(".//div[@class=\"content\"]").InnerText.Trim().Replace("\n      ", ""), replyContainerUserAuthor, DateTime.Parse(replyContainerInfo.SelectSingleNode(".//span[@class=\"time\"]").Attributes["title"].Value));

                            Comment r = new Comment(replyContainerUserScratchComment.id, replyContainerUserScratchComment, Residence.Studio, studioId.ToString(), true, new List<Comment>());

                            replies.Add(r);

                            comments.Add(r);
                        }
                    }

                    Comment c = new Comment(scratchComment.id, scratchComment, Residence.Studio, studioId.ToString(), false, replies);

                    comments.Add(c);
                }
            }

            List<int> commentIds = comments.Select(comment => comment.commentId).ToList();

            List<Comment> dbComments = commentService.GetMany(commentIds);

            List<Comment> workingComments = new List<Comment>();

            foreach (Comment comment in comments)
            {
                workingComments.Add(new Comment
                {
                    _id = comment._id,
                    commentId = comment.commentId,
                    comment = comment.comment,
                    residence = comment.residence,
                    residenceId = comment.residenceId,
                    reactions = comment.reactions,
                    isPinned = comment.isPinned,
                    isReply = comment.isReply,
                    replies = comment.replies
                });
            }

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(() =>
            {
                foreach (Comment comment in workingComments)
                {
                    if (dbComments.Find(dbComment => dbComment.commentId == comment.commentId) == null)
                    {
                        commentService.Create(comment);
                    }
                    else
                    {
                        Comment c = dbComments.Find(dbComment => dbComment.commentId == comment.commentId);
                        c.residence = comment.residence;
                        c.residenceId = comment.residenceId;
                        c.replies = comment.replies;
                        commentService.Update(comment.commentId, c);
                    }
                }
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            foreach (Comment dbComment in dbComments)
            {
                Comment c = dbComment;
                c.replies = comments.Find(comment => comment.commentId == dbComment.commentId).replies;
                comments[comments.IndexOf(comments.Find(comment => comment.commentId == dbComment.commentId))] = c;
            }

            return Ok(JsonConvert.SerializeObject(comments));
        }

        /// <summary>
        /// React to a comment
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut("{commentId}/reactions")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult PutReaction(int commentId, string reaction)
        {
            if (reactionService.Get(reaction) != null)
            {
                User user = userService.Get(HttpContext.User.Claims.ToList().Find(claim => claim.Type == "username").Value);

                if (user.isBanned)
                {
                    return Forbid();
                }

                Comment comment = commentService.Get(commentId);

                if (comment == null)
                {
                    comment = commentService.Create(new Comment(commentId, null, Residence.Project, "", false, new List<Comment>()));
                }

                if (comment.reactions == null)
                {
                    comment.reactions = new List<UserReaction>();
                }

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

        /*[HttpPut("{projectId}/{commentId}/pin")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

                List<Comment> replies = new List<Comment>();

                foreach (ScratchComment scratchComment in scratchCommentReplies)
                {
                    replies.Add(commentService.Create(new Comment(scratchComment.id, scratchComment, Residence.Project, projectId.ToString(), true, new List<Comment>())));
                }

                comment = commentService.Create(new Comment(commentId, JsonConvert.DeserializeObject<ScratchComment>(await response.Content.ReadAsStringAsync()), Residence.Project, projectId.ToString(), false, replies));
            }
            else
            {
                comment = commentService.Get(commentId);
            }

            comment.isPinned = pin;

            commentService.Update(commentId, comment);

            return Accepted();
        }*/

        /// <summary>
        /// Get all the stars for the logged in user
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("stars")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult GetStars()
        {
            User user = userService.Get(HttpContext.User.Claims.ToList().Find(claim => claim.Type == "username").Value);

            if (user.isBanned)
            {
                return Forbid();
            }

            return Ok(JsonConvert.SerializeObject(user.stars));
        }

        [HttpGet("query/{query}")]
        public ActionResult QueryComments(string query)
        {
            List<Comment> result = commentSearchService.QueryComments(JsonConvert.DeserializeObject<List<CommentSearchRequirement>>(query));
            return Ok(result);
        }
        
        /// <summary>
        /// Star/unstar a comment
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut("{commentId}/stars")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult StarComment(int commentId)
        {
            User user = userService.Get(HttpContext.User.Claims.ToList().Find(claim => claim.Type == "username").Value);

            if (user.isBanned)
            {
                return Forbid();
            }

            user.stars.Add(commentId);

            return Accepted();
        }
    }
}