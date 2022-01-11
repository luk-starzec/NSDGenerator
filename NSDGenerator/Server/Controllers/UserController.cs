using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NSDGenerator.Server.Repo;
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
    [ApiController, Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IDbRepo dbRepo;

        public UserController(IOptions<JwtSettings> options, IDbRepo dbRepo)
        {
            _jwtSettings = options.Value;
            this.dbRepo = dbRepo;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto login)
        {
            var result = login.Email == "user@starzec.net" && login.Password == "user1"
                || login.Email == "guest@starzec.net" && login.Password == "guest";

            if (!result)
                return BadRequest(new LoginResult(false, Error: "Email or password are invalid."));

            var stringToken = GetStringToken(login.Email);

            return Ok(new LoginResult(true, Token: stringToken));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto register)
        {
            var registrationCode = register.RegistrationCode;
            if (string.IsNullOrEmpty(registrationCode))
                return BadRequest(new RegisterResult(false, "Registration code required"));

            var error = await dbRepo.RegisterUserAsync(register);

            if (!string.IsNullOrEmpty(error))
                return BadRequest(new RegisterResult(false, error));

            var token = GetStringToken(register.Email);

            return Ok(new RegisterResult(true, Token: token));
        }

        private string GetStringToken(string name)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, name),
            };

            if (name.StartsWith("guest"))
                claims.Add(new Claim(ClaimTypes.Role, "Viewer"));

            if (name.StartsWith("User"))
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
            return stringToken;
        }
    }
}
