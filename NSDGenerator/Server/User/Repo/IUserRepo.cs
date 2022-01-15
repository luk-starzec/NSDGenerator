using NSDGenerator.Shared.User;
using System.Threading.Tasks;

namespace NSDGenerator.Server.User.Repo;

public interface IUserRepo
{
    Task<string> RegisterUserAsync(RegisterDto register);
    Task<bool> VerifyUserAsync(LoginDto user);
}
