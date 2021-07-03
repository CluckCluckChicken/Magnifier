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

        /// <summary>
        /// Get all the available reactions
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetReactions()
        {
            return Ok(reactionService.Get());
        }

        /// <summary>
        /// Get a reaction by its name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult GetReaction(string name)
        {
            Reaction reaction = reactionService.Get(name);

            if (reaction == null)
            {
                return NotFound("that reaction doesnt exist");
                
            }
            else
            {
                return Ok(reaction);
            }
        }

        /// <summary>
        /// Create a new reaction
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("{name}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
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

        /// <summary>
        /// Deletes a reaction
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpDelete("{name}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteReaction(string name)
        {
            bool admin = bool.Parse(HttpContext.User.Claims.ToList().Find(claim => claim.Type == "admin").Value);

            if (!admin)
            {
                return Unauthorized("nice try mate");
            }

            if (reactionService.Get(name) == null)
            {
                return NotFound("that reaction doesnt exist");
            }
            else
            {
                reactionService.Remove(reactionService.Get(name));

                return Accepted();
            }
        }
    }
}