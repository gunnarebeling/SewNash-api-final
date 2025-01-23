

namespace SewNash.Models.DTOs;

public class AvailabilityForClassDTO
{
    public int Id { get; set; }
   
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<DayForAvailabilityDTO> DaysAndTimes { get; set; }


}
public class AvailabilityPostDTO
{
    public int SewClass { get; set; }
    public List<DateTime> DateRange { get; set; }
    public List<DayForAvailabilityPostDTO> Days { get; set; }
    public List<int> Employees { get; set; }
}
