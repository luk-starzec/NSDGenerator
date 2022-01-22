using System.ComponentModel.DataAnnotations;

namespace NSDGenerator.Client.ViewModels;

public class LogInVM
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}
