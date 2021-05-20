using HtmlAgilityPack;
using Magnifier.Services;
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

        private Uri authProject = new Uri("https://api.scratch.mit.edu/users/potatophant/projects/529697303/comments");

        public AuthController(JwtAuthService _jwtAuthService, AuthCodeService _authCodeService)
        {
            jwtAuthService = _jwtAuthService;
            authCodeService = _authCodeService;
        }

        [HttpGet("code")]
        public ActionResult GenerateCode()
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            int len = 36;

            Random rnd = new Random();
            StringBuilder b = new StringBuilder(len);
            for (int i = 0; i < len; i++)
            {
                b.Append(chars[rnd.Next(chars.Length)]);
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
                        comments.Add(new ScratchComment(jsonComment.id, jsonComment.content, jsonComment.author));
                    }

                    string token = "";

                    foreach (ScratchComment comment in comments)
                    {
                        if (comment.content == code)
                        {
                            authCodeService.Update(code, new AuthCode(code, true));
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
    }
}