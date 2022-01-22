using NSDGenerator.Shared.User;
using System.Threading.Tasks;

namespace NSDGenerator.Server.User.Repo;

public interface IUserRepo
{
    Task<string> RegisterUserAsync(RegisterDTO register);
    Task<bool> VerifyUserAsync(LoginDTO user);
}
