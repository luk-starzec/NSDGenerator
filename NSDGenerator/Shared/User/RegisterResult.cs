namespace NSDGenerator.Shared.User;
public record RegisterResult(bool IsSuccessful, string Error = null, string Token = null);
