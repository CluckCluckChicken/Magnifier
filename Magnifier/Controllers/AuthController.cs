using HtmlAgilityPack;
using Magnifier.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Magnifier.Models
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtAuthService jwtAuthService;
        private readonly AuthCodeService authCodeService;
        private readonly UserService userService;

        private Uri authProject = new Uri("https://api.scratch.mit.edu/users/furrycat-auth/projects/534514916/comments");

        public AuthController(JwtAuthService _jwtAuthService, AuthCodeService _authCodeService, UserService _userService)
        {
            jwtAuthService = _jwtAuthService;
            authCodeService = _authCodeService;
            userService = _userService;
        }

        [HttpGet("code")]
        public ActionResult GenerateCode()
        {
            string consonants = "BCDFGHJKLMNPQRSTVWXYZbcdfghjklmnpqrstvwxyz";
            int len = 36;

            Random rnd = new Random();
            StringBuilder b = new StringBuilder(len);
            for (int i = 0; i < len; i++)
            {
                b.Append(consonants[rnd.Next(consonants.Length)]);
            }
            string result = b.ToString();

            authCodeService.Create(new AuthCode(result));
            return Ok(result);
        }

        [HttpGet("token")]
        public async Task<ActionResult> GetTokenAsync(string code)
        {
            foreach (AuthCode authCode in authCodeService.Get())
            {
                if (authCode.code == code && authCode.hasBeenUsed == false)
                {
                    HttpClient client = new HttpClient();
                    var response = await client.GetAsync(authProject);
                    var data = await response.Content.ReadAsStringAsync();

                    dynamic apiComments = JsonConvert.DeserializeObject<List<ScratchComment>>(data);

                    List<ScratchComment> comments = new List<ScratchComment>();

                    foreach (ScratchComment jsonComment in apiComments)
                    {
                        comments.Add(new ScratchComment(jsonComment.id, jsonComment.content, jsonComment.author, jsonComment.datetime_created));
                    }

                    string token = "";

                    foreach (ScratchComment comment in comments)
                    {
                        if (comment.content == code)
                        {

                            authCodeService.Update(code, new AuthCode(code, true));

                            User user = userService.Get(comment.author.username);

                            if (jwtAuthService.GetBannedUsers().Contains(comment.author.username))
                            {
                                return Forbid();
                            }

                            if (user == null)
                            {
                                user = userService.Create(new User(comment.author.username, comment.author, comment.author.username == "potatophant"));
                            }
                            else
                            {
                                if (userService.Get(comment.author.username).isBanned)
                                {
                                    return Forbid();
                                }
                            }

                            user.lastLogin = DateTime.Now;

                            userService.Update(user.username, user);

                            token = jwtAuthService.GenerateJwt(code, comment.author.username, comment.author.username == "potatophant");
                        }
                    }

                    if (token == "")
                    {
                        return BadRequest();
                    }
                    else
                    {
                        return Ok(token);
                    }
                }
            }

            return Unauthorized();
        }

        [HttpGet("user")]
        [Authorize]
        public ActionResult GetUser()
        {
            User user = userService.Get(HttpContext.User.Claims.ToList().Find(claim => claim.Type == "username").Value);

            if (user.isBanned)
            {
                return Forbid();
            }

            if (user != null)
            {
                return Ok(JsonConvert.SerializeObject(user));
            }

            return NotFound();
        }

        [HttpGet("settings")]
        [Authorize]
        public ActionResult GetSettings()
        {
            User user = userService.Get(HttpContext.User.Claims.ToList().Find(claim => claim.Type == "username").Value);

            if (user.isBanned)
            {
                return Forbid();
            }

            if (user != null)
            {
                return Ok(JsonConvert.SerializeObject(user.settings));
            }

            return NotFound();
        }

        [HttpPut("settings")]
        [Authorize]
        public ActionResult UpdateSettings(string settings)
        {
            User user = userService.Get(HttpContext.User.Claims.ToList().Find(claim => claim.Type == "username").Value);

            Settings deserialized = JsonConvert.DeserializeObject<Settings>(settings);

            if (user.isBanned)
            {
                return Forbid();
            }

            if (user != null)
            {
                user.settings = deserialized;

                userService.Update(user.username, user);

                return Accepted();
            }

            return NotFound();
        }
    }
}