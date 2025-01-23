using System.ComponentModel.DataAnnotations;


namespace SewNash.Models;

public class Day
{
    public int Id { get; set; }
    [Required]
    public string DayOfWeek { get; set; }

}