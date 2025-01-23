using AutoMapper;
using SewNash.Models;
using SewNash.Models.DTOs;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {

        
        CreateMap<Employee, EmployeeDTO>();
        CreateMap<Employee, SimpleEmployeeDTO>();
        CreateMap<SewClass, SewClassDTO>();
        CreateMap<PostClassDTO, SewClass>();
        CreateMap<SewClass, SewClassForSessionDTO>();
        CreateMap<DayForAvailability, DayForAvailabilityDTO>();
        CreateMap<Time, TimeDTO>();
        CreateMap<Day, DayDTO>();
        CreateMap<DayForAvailabilityPostDTO, Day>();
        CreateMap<Session, SessionForClassBookingDTO>();
        CreateMap<Session, SessionDTO>();
        CreateMap<Booking, BookingForSessionDTO>();
        CreateMap<BookingForPostDTO, Booking>();
        CreateMap<Photo, PhotoDTO>();
        

    }
        
    
}