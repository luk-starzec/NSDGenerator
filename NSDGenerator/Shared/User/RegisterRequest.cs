namespace NSDGenerator.Shared.User;

public record RegisterRequest(string Email, string Password, string RegistrationCode);
