using System.ComponentModel.DataAnnotations;
namespace SewNash.Models;

public class User
{
    public Guid Id { get; set; } 

    [Required, EmailAddress]
    public string Email { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    [Required, MaxLength(50)]
    public string Role { get; set; } = "User";

    public Employee Employee { get; set; }
}