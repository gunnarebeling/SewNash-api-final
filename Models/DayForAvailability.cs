using System.ComponentModel.DataAnnotations;


namespace SewNash.Models;

public class DayForAvailability
{
    public int Id { get; set; }
    [Required]
    public int DayId { get; set; }
    public Day Day {get; set;}
    public List<Time> Times { get; set; }

}