﻿using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace NSDGenerator.Server.User.Models;

public class JwtSettings
{
    public const string JwtSettingsKey = "JwtSettings";

    public string SecurityKey { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int ExpiryInDays { get; set; }

    public SymmetricSecurityKey SigningKey => new(Encoding.UTF8.GetBytes(SecurityKey));
}
