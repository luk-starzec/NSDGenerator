namespace NSDGenerator.Shared.Login;
public record RegisterResult(bool IsSuccessful, string Error = null, string Token = null);
