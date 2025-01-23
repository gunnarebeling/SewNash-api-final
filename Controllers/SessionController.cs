using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SewNash.Data;
using Microsoft.EntityFrameworkCore;
using SewNash.Models;
using SewNash.Models.DTOs;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using Amazon.S3;


namespace SewNash.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SessionController :  PhotoParent
{
    private SewNashDbContext _dbContext;
    private IMapper _mapper;

    public SessionController(SewNashDbContext context, IMapper mapper, IAmazonS3 s3Client)
            : base(s3Client) // Pass the dependencies to the PhotoParent constructor
        {
            _dbContext = context;
            _mapper = mapper;
        }

    [HttpGet]
    public IActionResult GetAll()
    {
        List<SessionDTO> sessions = _dbContext.Sessions.ProjectTo<SessionDTO>(_mapper.ConfigurationProvider).ToList();
        sessions.ForEach(s => ConvertFileKey(s.SewClass));
        return Ok(sessions); 
        
        
    }

    [HttpGet("class/{classId}")]
    public IActionResult GetDetailsForClass(string classId)
    {
       return Ok(  _dbContext.Sessions.ProjectTo<SessionDTO>(_mapper.ConfigurationProvider).Where(s => s.SewClassId == int.Parse(classId)));
        
    }

    [HttpGet("{sessionId}")]
    public IActionResult GetSessionById(string sessionId)
    {
       return Ok(  _dbContext.Sessions.ProjectTo<SessionDTO>(_mapper.ConfigurationProvider).SingleOrDefault(s => s.Id == int.Parse(sessionId)));
        
    }

    [HttpDelete("{sessionId}")]
    [Authorize]
    public IActionResult Delete(string sessionId)
    {
       Session session = _dbContext.Sessions.SingleOrDefault(s => s.Id == int.Parse(sessionId));
       _dbContext.Sessions.Remove(session);
       _dbContext.SaveChanges();
       return NoContent();
    }
    [HttpPut("{sessionId}")]
    [Authorize]
    public IActionResult toggleOpen(string sessionId)
    {
       Session session = _dbContext.Sessions.SingleOrDefault(s => s.Id == int.Parse(sessionId));
       session.Open = !session.Open;
       _dbContext.SaveChanges();
       return NoContent();
    }

    [HttpGet("date")]
    public IActionResult GetSesionsByDate([FromQuery] DateTime date)
    {
      List<Session> sessions = _dbContext.Sessions.ToList();
      if (date != null)
      {
         sessions = sessions.Where(s => s.DateTime == date).ToList();
         
      }
         return Ok( _mapper.Map<SessionDTO>(sessions));
        
    }

    

}