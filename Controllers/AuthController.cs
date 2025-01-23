
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using SewNash.Models;
using SewNash.Models.DTOs;
using SewNash.Data;
using System.Net.Sockets;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

namespace SewNash.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private SewNashDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public AuthController(SewNashDbContext context, IConfiguration configuration)
    {
        _dbContext = context;
        _configuration = configuration;
    }
    

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegistrationDTO registration)
    {
        
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registration.Password);
        var user = new User
        {
            Email = registration.Email,
            PasswordHash= hashedPassword,
            Role = "User",
            Employee = new Employee
            {
                FirstName = registration.FirstName,
                LastName = registration.LastName,
                PhoneNumber = registration.PhoneNumber,
                Email = registration.Email
            }
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return Ok("User created successfully");
    }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            User user = await _dbContext.Users.Include(u => u.Employee).SingleOrDefaultAsync(u => u.Email == dto.Email);
                                          

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid email or password.");

            var token = GenerateJwtToken(user);

            return Ok(new { Token = token });
        }
        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var nonCodedKey = Environment.GetEnvironmentVariable("JWT_KEY");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(nonCodedKey!));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(double.Parse(jwtSettings["ExpireHours"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
            return Unauthorized("User ID not found in token.");

        var user = await _dbContext.Users
            .Include(u => u.Employee)
            .FirstOrDefaultAsync(u => u.Id.ToString() == userId);

        if (user == null)
            return NotFound("User not found.");

        return Ok(new EmployeeDTO
        {
            Id = user.Employee.Id,
            FirstName = user.Employee.FirstName,
            LastName = user.Employee.LastName,
            Email = user.Employee.Email,
            PhoneNumber = user.Employee.PhoneNumber,
            Role = user.Role

        });
    }

    [HttpGet("verify-user")]
    [Authorize]
    public IActionResult VerifyUser()
    {
        return Ok("You are a User.");
    }


    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // Client must clear the token from session storage
        return Ok("Logged out successfully. Please clear your token.");
    }

    [HttpGet("verify-admin")]
    [Authorize(Roles = "Admin")]
    public IActionResult VerifyAdmin()
    {
        return Ok("You are an Admin.");
    }

}