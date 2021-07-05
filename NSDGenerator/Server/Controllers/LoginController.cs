using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NSDGenerator.Shared.Login;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NSDGenerator.Server.Controllers
{
    [ApiController, Route("api")]
    public class LoginController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        public LoginController(IOptions<JwtSettings> options)
        {
            _jwtSettings = options.Value;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            var result = login.Email == "user@starzec.net" && login.Password == "user1"
                || login.Email == "guest@starzec.net" && login.Password == "guest";

            if (!result)
                return BadRequest(new LoginResult(false, Error: "Email or password are invalid."));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, login.Email)
            };

            if (login.Email.StartsWith("guest"))
                claims.Add(new Claim(ClaimTypes.Role, "Viewer"));

            if (login.Email.StartsWith("User"))
                claims.Add(new Claim(ClaimTypes.Role, "Editor"));

            var creds = new SigningCredentials(_jwtSettings.SigningKey, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.Now.AddDays(_jwtSettings.ExpiryInDays);

            var token = new JwtSecurityToken(
                _jwtSettings.Issuer,
                _jwtSettings.Audience,
                claims,
                expires: expiry,
                signingCredentials: creds
            );
            var stringToken = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new LoginResult(true, Token: stringToken));
        }

    }
}
