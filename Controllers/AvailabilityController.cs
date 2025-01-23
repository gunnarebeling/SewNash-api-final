using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SewNash.Data;
using Microsoft.EntityFrameworkCore;
using SewNash.Models;
using SewNash.Models.DTOs;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using System.Net.WebSockets;

namespace SewNash.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AvailabilityController : ControllerBase
{
    private SewNashDbContext _dbContext;
    private IMapper _mapper;

    public AvailabilityController(SewNashDbContext context, IMapper mapper)
    {
        _dbContext = context;
        _mapper = mapper;
    }

    [HttpPost]
    [Authorize]
    public IActionResult Get([FromBody] AvailabilityPostDTO availabilityPost)
    {  
         List<Session> TotalSessions = new List<Session>();
        for (DateTime day = availabilityPost.DateRange[0]; day <= availabilityPost.DateRange[1]; day = day.AddDays(1))
        {
            if (availabilityPost.Days.Any(d => d.DayOfWeek == day.DayOfWeek.ToString()))
            {
                var selectDay = availabilityPost.Days.SingleOrDefault((d => d.DayOfWeek == day.DayOfWeek.ToString()));
                Day theDay = _dbContext.Days.SingleOrDefault(d => d.DayOfWeek == selectDay.DayOfWeek);
                List<Employee> employees = _dbContext.Employees.Where(e => availabilityPost.Employees.Contains(e.Id)).ToList();

                var daySessions = selectDay.Times.Select(time => new Session
            {
                SewClassId = availabilityPost.SewClass,
                DateTime = day,
                DayId = theDay.Id,
                TimeId = time,
                Employees = employees,
                Open = true
            }).ToList();
                daySessions.ForEach(d => TotalSessions.Add(d));
                

                
            }

        }
        _dbContext.Sessions.AddRange(TotalSessions);
        _dbContext.SaveChanges();
        return Ok();

    }



    

}