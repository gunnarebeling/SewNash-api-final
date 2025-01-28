using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SewNash.Data;
using Microsoft.EntityFrameworkCore;
using SewNash.Models;
using SewNash.Models.DTOs;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using System.Net.Mail;
using System.Net;
using RedLockNet;
using RedLockNet.SERedis.Configuration;
using RedLockNet.SERedis;
using StackExchange.Redis;

namespace SewNash.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private SewNashDbContext _dbContext;
    private IMapper _mapper;
    private readonly ILogger<BookingController> _logger;
    private readonly IConnectionMultiplexer _redis;
    private static IDistributedLockFactory _redLockFactory;
    private static IRedLock _currentLock;
    

    public BookingController(SewNashDbContext context, IMapper mapper, ILogger<BookingController> logger, IConnectionMultiplexer redis)
    {
        _dbContext = context;
        _mapper = mapper;
        var redisEndpoints = new[] { new RedLockEndPoint { EndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6379) } };
        _redLockFactory = RedLockFactory.Create(redisEndpoints);
        _logger = logger;
        _redis = redis;
    }

    [HttpGet("set-lock/{sessionId}")]
    public async Task<IActionResult> SetLock(string sessionId)
    {
        var resource = $"lock:session:{sessionId}";
        var expiry = TimeSpan.FromSeconds(10);
        bool lockExists = await _redis.GetDatabase().KeyExistsAsync(resource);
        if (lockExists)
        {
            _logger.LogError("Lock already exists");
            return StatusCode(409);
        }

        using (var redLock = await _redLockFactory.CreateLockAsync(resource, expiry))
        {
            
            if (redLock.IsAcquired)
            {
                _logger.LogWarning($"Lock acquired for session {sessionId}");
                _currentLock = redLock;
                var db = _redis.GetDatabase();
                db.SetAdd(resource, "lock");

                _logger.LogWarning($"Lock:{_currentLock}");
                return Ok();                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 
            }
            else
            {
                _logger.LogError("Lock not acquired");
                return StatusCode(409);
            }
        }
    }


    [HttpPost]
    public async Task<IActionResult> PostBooking([FromBody] BookingForPostDTO booking)
    {
        var resource = $"lock:session:{booking.SessionId}";
        _logger.LogWarning($"Lock:{resource}");
        var db = _redis.GetDatabase();
        var lockExists = await db.KeyExistsAsync(resource);
        _logger.LogWarning($"Lock:{_currentLock}");
        if (!lockExists)
        {
            
            _logger.LogError("Lock does not exist");
            return StatusCode(409, "Lock does not exist");
        }
       using var transaction = await _dbContext.Database.BeginTransactionAsync();
       try
       {
            Booking PostBooking = _mapper.Map<Booking>(booking);
            PostBooking.DateBooked = DateTime.Now;
            _dbContext.Bookings.Add(PostBooking);
            _dbContext.SaveChanges();

            // Commit the transaction

            SessionDTO session = _dbContext.Sessions.ProjectTo<SessionDTO>(_mapper.ConfigurationProvider).SingleOrDefault(s => s.Id == booking.SessionId);
            DateTime date = session.DateTime;
            string sewClass = session.SewClass.Name;
            string americanDateFormat = date.ToString("MM/dd/yyy");
            string day = session.Day.DayOfWeek;
            string Time = session.Time.StartTime;
            string DateAndTime = $"{americanDateFormat} {day} {Time}";
            string name = PostBooking.Name; 
            await SendEmailAsync(PostBooking.Email, name, sewClass, DateAndTime);
            await transaction.CommitAsync();

            // Release the lock
            _currentLock.Dispose();
            _currentLock = null;
            db.KeyDelete(resource);
            _logger.LogInformation($"Lock released for session {booking.SessionId}");

            _logger.LogInformation($"Booking created: {PostBooking}");
            return Created($"/booking/{PostBooking.Id}", PostBooking);
       }
       catch (System.Exception ex)
       {
            _logger.LogError(ex, "An error occurred while creating a booking");
             await transaction.RollbackAsync();
            return StatusCode(500, ex.Message);
       }
    }

    private async Task SendEmailAsync(string email, string name,string sewClass, string bookingDateTime)
        {
            string smtpServer = Environment.GetEnvironmentVariable("SMTP_SERVER"); // Replace with your SMTP server
            int smtpPort = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT"));                  // Common SMTP port
            string fromEmail = Environment.GetEnvironmentVariable("EMAIL_ADDRESS"); // Your email
            string emailPassword = Environment.GetEnvironmentVariable("EMAIL_PASSWORD"); // Your email password (use environment variables in production)

            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(fromEmail, "MockSewNash");
                mail.To.Add(email);
                mail.Subject = "Booking Confirmation";
                mail.Body = $"Thank you {name} for booking the {sewClass} with us! Your class is scheduled for {bookingDateTime}.";
                mail.IsBodyHtml = false;

                using (SmtpClient smtp = new SmtpClient(smtpServer, smtpPort))
                {
                    smtp.Credentials = new NetworkCredential(fromEmail, emailPassword);
                    smtp.EnableSsl = true;

                    await smtp.SendMailAsync(mail); // Asynchronous email sending
                }
            }
        }

    [HttpDelete("{id}")]
    [Authorize]
    public IActionResult Delete(string id)
    {
        Booking booking = _dbContext.Bookings.SingleOrDefault(b => b.Id == int.Parse(id));
        if (booking == default)
        {
            return NotFound();
            
        }
        _dbContext.Bookings.Remove(booking);
        _dbContext.SaveChanges();
        return NoContent();
    }

    [HttpPut("{id}")]
    [Authorize]
    public IActionResult Update(string id,[FromBody] BookingUpdateDTO bookingUpdate)
    {
        Booking booking = _dbContext.Bookings.SingleOrDefault(b => b.Id == int.Parse(id));

        if (booking == default)
        {
            return NotFound();
            
        }
        booking.Name = bookingUpdate.Name;
        booking.PhoneNumber = bookingUpdate.PhoneNumber;
        booking.Occupancy = bookingUpdate.Occupancy;
        _dbContext.SaveChanges();
        return NoContent();
    }


    

}