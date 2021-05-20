using Magnifier.Models;
using Magnifier.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magnifier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReactionsController : ControllerBase
    {
        private readonly CommentService commentService;
        private readonly ReactionService reactionService;

        public ReactionsController(CommentService _commentService, ReactionService _reactionService)
        {
            commentService = _commentService;
            reactionService = _reactionService;
        }

        [HttpGet]
        public ActionResult GetReactions()
        {
            return Ok(reactionService.Get());
        }

        [HttpPost("{name}")]
        [Authorize]
        public ActionResult CreateReaction(string name, string emoji)
        {
            bool admin = bool.Parse(HttpContext.User.Claims.ToList().Find(claim => claim.Type == "admin").Value);

            if (!admin)
            {
                return Unauthorized("nice try mate");
            }

            if (reactionService.Get(name) == null)
            {
                reactionService.Create(new Reaction(name, emoji));

                return Accepted();
            }
            else
            {
                return Forbid("that name is already taken");
            }
        }

        [HttpDelete("{name}")]
        [Authorize]
        public ActionResult DeleteReaction(string name)
        {
            bool admin = bool.Parse(HttpContext.User.Claims.ToList().Find(claim => claim.Type == "admin").Value);

            if (!admin)
            {
                return Unauthorized("nice try mate");
            }

            if (reactionService.Get(name) != null)
            {
                reactionService.Remove(reactionService.Get(name));

                return Accepted();
            }
            else
            {
                return NotFound("that reaction doesnt exist");
            }
        }
    }
}