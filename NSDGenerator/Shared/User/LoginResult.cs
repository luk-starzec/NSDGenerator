namespace NSDGenerator.Shared.User;

public record LoginResult(bool IsSuccessful, string Error = null, string Token = null);
