using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SewNash.Data;
using Microsoft.EntityFrameworkCore;
using SewNash.Models;
using SewNash.Models.DTOs;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using System.Net.WebSockets;
using StackExchange.Redis;
using System.Text.Json;

namespace SewNash.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AvailabilityController : ControllerBase
{
    private SewNashDbContext _dbContext;
    private IMapper _mapper;
    private IConnectionMultiplexer _redis;

    public AvailabilityController(SewNashDbContext context, IMapper mapper, IConnectionMultiplexer redis)
    {
        _dbContext = context;
        _mapper = mapper;
        _redis = redis;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Get([FromBody] AvailabilityPostDTO availabilityPost)
    {  
         List<Session> TotalSessions = new List<Session>();
         List<RedisSession> TotalRedisSessions = new List<RedisSession>();
         var redisDb = _redis.GetDatabase();
         var batch = redisDb.CreateBatch();
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
         await _dbContext.SaveChangesAsync();
        List<Session> newSessions = _dbContext.Sessions.Include(s => s.SewClass).Where(s => TotalSessions.Contains(s)).ToList();
        TotalRedisSessions = newSessions.Select(s => _mapper.Map<RedisSession>(s)).ToList();
        
        TotalRedisSessions.ForEach(session =>
        {
            var key = $"session:{session.Id}";
            batch.StringSetAsync(key, JsonSerializer.Serialize(session));
        });
        batch.Execute();
        return Ok();

    }



    

}