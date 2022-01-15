using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSDGenerator.Server.DbData;
using NSDGenerator.Server.User.Helpers;
using NSDGenerator.Shared.User;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NSDGenerator.Server.User.Repo
{
    public class UserRepo : IUserRepo
    {
        private readonly ILogger<UserRepo> logger;
        private readonly NsdContext context;
        private readonly IPasswordHasher passwordHasher;

        public UserRepo(ILogger<UserRepo> logger, NsdContext context, IPasswordHasher passwordHasher)
        {
            this.logger = logger;
            this.context = context;
            this.passwordHasher = passwordHasher;
        }

        public async Task<string> RegisterUserAsync(RegisterDto register)
        {
            var code = await context.RegistrationCodes
                .Where(r => r.Code == register.RegistrationCode)
                .Where(r => r.IsActive)
                .Where(r => !r.ValidTo.HasValue || r.ValidTo >= DateTime.Now)
                .SingleOrDefaultAsync();

            if (code is null)
                return "Invalid registration code";

            var existingUser = await context.Users.AnyAsync(r => r.Name == register.Email);

            if (existingUser)
                return $"User {register.Email} already exists";

            var user = new DbData.User
            {
                Name = register.Email,
                Password = passwordHasher.Hash(register.Password),
                IsEnabled = true,
                Created = DateTime.Now,
            };
            context.Users.Add(user);

            code.IsActive = false;

            await context.SaveChangesAsync();

            return null;
        }

        public async Task<bool> VerifyUserAsync(LoginDto user)
        {
            var dbUser = await context.Users.SingleOrDefaultAsync(r => r.Name == user.Email);

            if (dbUser?.Password is null)
                return false;

            var (verified, needsUpgrade) = passwordHasher.Check(dbUser.Password, user.Password);
            return verified;
        }
    }
}
