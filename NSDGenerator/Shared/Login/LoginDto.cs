using System.ComponentModel.DataAnnotations;

namespace NSDGenerator.Shared.Login;

public class LoginDto
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}
