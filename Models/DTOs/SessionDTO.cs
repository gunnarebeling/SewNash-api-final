
namespace SewNash.Models.DTOs;

public class SessionForClassBookingDTO
{
    public int Id { get; set; }
    public DateTime DateTime { get; set; }
    public int SewClassId { get; set; }
    public int TimeId { get; set; }
    public TimeDTO Time { get; set; }
    public int DayId { get; set; }
    public List<SimpleEmployeeDTO> Employees { get; set; }
    
    public DayDTO Day { get; set; }
    public bool Open { get; set; }

}
public class SessionDTO
{
    public int Id { get; set; }
    public DateTime DateTime { get; set; }
    public int SewClassId { get; set; }
    public SewClassForSessionDTO SewClass { get; set; }
    public int TimeId { get; set; }
    public TimeDTO Time { get; set; }
    public int DayId { get; set; }
    
    public DayDTO Day { get; set; }
    public List<BookingForSessionDTO> Bookings { get; set; }
    public List<SimpleEmployeeDTO> Employees { get; set; }
    public int TotalPeople 
    { 
        get 
        {
            int totalPeople = Bookings.Aggregate(0, (total, b) => {
                total += b.Occupancy;
                return total;
            });
            return totalPeople;
        }
    }
    public decimal TotalAmount
    {
        get
        {
            decimal total = Bookings.Aggregate(0m, (decimal total, BookingForSessionDTO b) => 
            {  
                total += b.Occupancy * SewClass.PricePerPerson;
                return total;

            });
            return total;
        }
    }
    private bool _isOpen;
    public bool Open 
    { 
        get
    {
        if (_isOpen) // if it's true, apply the condition
        {
            return TotalPeople < SewClass.MaxPeople;
        }
        else
        {
            return false; // return false when it was set to false
        }
    }
    set
    {
        _isOpen = value; // Allow manually setting the value to true or false
    }
    }

    

}
