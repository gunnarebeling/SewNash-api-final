using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SewNash.Models;

public class Employee
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string PhoneNumber {get; set;}
    [Required]
    
    [ForeignKey("User")]
    public Guid UserId { get; set; }
    public User User { get; set; }    
    public List<Session> Sessions { get; set; }
}