using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using NSDGenerator.Server.DbData;
using NSDGenerator.Server.Diagram.Helpers;
using NSDGenerator.Server.Diagram.Repo;
using NSDGenerator.Server.User.Helpers;
using NSDGenerator.Server.User.Models;
using NSDGenerator.Server.User.Repo;
using System;

namespace NSDGenerator.Server;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<NsdContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("NsdDatabase")));

        services.AddScoped<IDiagramRepo, DiagramRepo>();
        services.AddScoped<IDtoConverter, DtoConverter>();

        services.Configure<HashingSettings>(Configuration.GetSection(HashingSettings.HashingOptionsKey));
        services.AddScoped<IUserRepo, UserRepo>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        services.Configure<JwtSettings>(Configuration.GetSection(JwtSettings.JwtSettingsKey));
        var jwtSettings = Configuration.GetSection(JwtSettings.JwtSettingsKey).Get<JwtSettings>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = jwtSettings.SigningKey,
                    ClockSkew = TimeSpan.Zero,
                    RequireExpirationTime = true,
                };
            });

        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapFallbackToFile("index.html");
        });
    }
}
