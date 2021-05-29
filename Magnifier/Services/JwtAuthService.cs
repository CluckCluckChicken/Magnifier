using Magnifier.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Magnifier.Services
{
    public class JwtAuthService
    {
        private readonly IJwtAuthSettings auth;
        private readonly UserService userService;

        public JwtAuthService(IJwtAuthSettings _auth, UserService _userService)
        {
            auth = _auth;
            userService = _userService;
        }

        public string GenerateJwt(string code, string username, bool isAdmin = false)
        {
            // this doesnt check if user is banned. you will need to check that beforehand.

            string privateKey = auth.PrivateKey;
            string issuer = auth.Issuer;
            string audience = auth.Audience;
            double lifetimeDays = auth.LifetimeDays;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(privateKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var claims = new List<Claim>();
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim("username", username));
            claims.Add(new Claim("admin", isAdmin.ToString()));

            var token = new JwtSecurityToken(issuer,
                            audience,
                            claims,
                            expires: DateTime.Now.AddDays(lifetimeDays),
                            signingCredentials: credentials);
            string jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        public string[] GetBannedUsers()
        {
            return new string[] { "dlpenguin4" };
        }
    }
}
