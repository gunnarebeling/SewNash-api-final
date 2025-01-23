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

namespace SewNash.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private SewNashDbContext _dbContext;
    private IMapper _mapper;

    public BookingController(SewNashDbContext context, IMapper mapper)
    {
        _dbContext = context;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> PostBooking([FromBody] BookingForPostDTO booking)
    {
        Booking PostBooking = _mapper.Map<Booking>(booking);
        PostBooking.DateBooked = DateTime.Now;
        _dbContext.Bookings.Add(PostBooking);
        _dbContext.SaveChanges();
        SessionDTO session = _dbContext.Sessions.ProjectTo<SessionDTO>(_mapper.ConfigurationProvider).SingleOrDefault(s => s.Id == booking.SessionId);
        DateTime date = session.DateTime;
        string sewClass = session.SewClass.Name;
        string americanDateFormat = date.ToString("MM/dd/yyy");
        string day = session.Day.DayOfWeek;
        string Time = session.Time.StartTime;
        string DateAndTime = $"{americanDateFormat} {day} {Time}";
        string name = PostBooking.Name; 
        await SendEmailAsync(PostBooking.Email, name, sewClass, DateAndTime);
        return Created($"/booking/{PostBooking.Id}", PostBooking);
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
    public IActionResult Update(string id, BookingUpdateDTO bookingUpdate)
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