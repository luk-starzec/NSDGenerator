using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NSDGenerator.Server.User.Models;
using NSDGenerator.Server.User.Repo;
using NSDGenerator.Shared.User;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NSDGenerator.Server.User
{
    [ApiController, Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IUserRepo _repo;

        public UserController(IOptions<JwtSettings> options, IUserRepo userRepo)
        {
            _jwtSettings = options.Value;
            _repo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            var isValid = await _repo.VerifyUserAsync(login);

            if (!isValid)
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

            var error = await _repo.RegisterUserAsync(register);

            if (!string.IsNullOrEmpty(error))
                return BadRequest(new RegisterResult(false, error));

            var token = GetStringToken(register.Email);

            return Ok(new RegisterResult(true, Token: token));
        }

        private string GetStringToken(string name)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, name) };

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
