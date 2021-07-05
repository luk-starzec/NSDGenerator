namespace NSDGenerator.Shared.Login
{
    public record LoginResult(bool IsSuccessful, string Error = null, string Token = null);
}
