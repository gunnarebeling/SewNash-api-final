using System.ComponentModel.DataAnnotations;
using SewNash.Models.DTOs;


namespace SewNash.Models;

public class Session
{
    public int Id { get; set; }
    [Required]
    public int SewClassId { get; set; }
    public SewClass SewClass { get; set; }
    [Required]
    public DateTime DateTime { get; set; }
    [Required]
    public int TimeId { get; set; }
    public Time Time { get; set; }
    [Required]
    public int DayId { get; set; }
    
    public Day Day { get; set; }
    [Required]
    public List<Booking> Bookings { get; set; }
    public List<Employee> Employees { get; set; }

    public bool Open { get; set; }

}

public class RedisSession
{
    public int Id { get; set; }
    public int SewClassId { get; set; }
    public SewClassForRedisDTO SewClass { get; set; }
    public DateTime DateTime { get; set; }
    public int TimeId { get; set; }
    public int DayId { get; set; }
    public List<BookingForPostDTO> Bookings { get; set; }
    public bool Open { get; set; }
    public bool Processing { get; set; } = false;
}