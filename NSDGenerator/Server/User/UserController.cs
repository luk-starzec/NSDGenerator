using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

namespace NSDGenerator.Server.User;

[ApiController, Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly JwtSettings _jwtSettings;
    private readonly IUserRepo _repo;
    private readonly ILogger<UserController> _logger;

    public UserController(IOptions<JwtSettings> options, IUserRepo userRepo, ILogger<UserController> logger)
    {
        _jwtSettings = options.Value;
        _repo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
        _logger = logger;
    }

    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest user)
    {
        var isValid = await _repo.VerifyUserAsync(user);

        if (!isValid)
            return BadRequest(new LoginResult(false, Error: "Email or password are invalid."));

        var stringToken = GetStringToken(user.Email);

        return stringToken is not null
            ? Ok(new LoginResult(true, Token: stringToken))
            : StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest register)
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
        try
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
        catch (Exception ex)
        {
            _logger.LogError("Method {Method} thrown exception: {Message}", nameof(GetStringToken), ex.Message);
            return null;
        }
    }
}
