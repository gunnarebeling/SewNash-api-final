using System.ComponentModel.DataAnnotations;


namespace SewNash.Models;

public class Booking
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public DateTime DateBooked {get; set;}
    [Required]
    public string PhoneNumber { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public int Occupancy { get; set; }
    [Required]
    public int SessionId { get; set; }
    public Session Session { get; set; }
    
   
}