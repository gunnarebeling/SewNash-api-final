

namespace SewNash.Models.DTOs;

public class DayForAvailabilityDTO
{
    public int Id { get; set; }

    public int DayId { get; set; }
    public DayDTO Day {get; set;}
    public List<TimeDTO> Times { get; set; }

}