using System.ComponentModel.DataAnnotations;

namespace NSDGenerator.Client.Models;

public class SignInModel
{
    [Required, EmailAddress]
    public string Email { get; set; }

    [Required, MinLength(8, ErrorMessage = "Password should be at least 8 characters long")]
    public string Password { get; set; }

    [Required, Compare(nameof(Password), ErrorMessage = "Fields Password and Confirm must be the same")]
    public string PasswordConfirmation { get; set; }

    [Required]
    public string RegistrationCode { get; set; }
}
