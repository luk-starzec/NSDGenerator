namespace NSDGenerator.Server.User.Models;

public class HashingSettings
{
    public const string HashingOptionsKey = "HashingSettings";

    public int Iterations { get; set; } = 10000;
}
