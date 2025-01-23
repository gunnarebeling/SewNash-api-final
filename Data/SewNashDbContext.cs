using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SewNash.Models;



namespace SewNash.Data;
public class SewNashDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    public DbSet<User> Users { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Time> Times { get; set; }
    public DbSet<Day> Days { get; set; }
    public DbSet<DayForAvailability> DayForAvailabilities { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<SewClass> SewClasses {get; set;}
    public DbSet<Photo> Photos { get; set; }
    

    public SewNashDbContext(DbContextOptions<SewNashDbContext> context, IConfiguration config) : base(context)
    {
        _configuration = config;
    }
       
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var adminId = Guid.Parse("f5b1f1c6-0e4f-4b0f-8b3b-1b3b8e1b1b1b");
        var johnDoeId = Guid.Parse("d2f2b67f-3a58-4c5e-9dbf-8be4d99b093b"); // Ensure consistency across seeding
        var adminHash = _configuration["AdminPassword"];
        var JohnDoeHash = _configuration["JohnDoePassword"];
        modelBuilder.Entity<User>().HasData(new User
            {
                Id = adminId,
                Email = "admina@strator.comx",
                PasswordHash = adminHash,
                Role = "Admin"
            });

        modelBuilder.Entity<Employee>().HasData(new Employee
            {
                Id = 1,
                FirstName = "Admin",
                LastName = "User",
                Email = "admina@strator.comx",
                PhoneNumber = "123-123-1234",
                UserId = adminId
            });
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = johnDoeId,
                Email = "JohnDoe@test.com",
                PasswordHash = JohnDoeHash,
                Role = "User"
            });

        modelBuilder.Entity<Employee>().HasData(new Employee
            {
                Id = 2,
                FirstName = "John",
                LastName = "Doe",
                Email = "JohnDoe@test.com",
                PhoneNumber = "222-222-2222",
                UserId = johnDoeId
            });



       
        
        modelBuilder.Entity<Time>().HasData(new Time[]
        {
            new Time { Id = 1, StartTime = "10:00 AM" },
            new Time { Id = 2, StartTime = "11:00 AM" },
            new Time { Id = 3, StartTime = "12:00 PM" },
            new Time { Id = 4, StartTime = "01:00 PM" },
            new Time { Id = 5, StartTime = "02:00 PM" },
            new Time { Id = 6, StartTime = "03:00 PM" },
            new Time { Id = 7, StartTime = "04:00 PM" },
            new Time { Id = 8, StartTime = "05:00 PM" },
            new Time { Id = 9, StartTime = "06:00 PM" },
            new Time { Id = 10, StartTime = "07:00 PM" },
            new Time { Id = 11, StartTime = "08:00 PM" }
        });
        modelBuilder.Entity<Day>().HasData(new Day[]
        {
            new Day { Id = 1, DayOfWeek = "Monday" },
            new Day { Id = 2, DayOfWeek = "Tuesday" },
            new Day { Id = 3, DayOfWeek = "Wednesday" },
            new Day { Id = 4, DayOfWeek = "Thursday" },
            new Day { Id = 5, DayOfWeek = "Friday" },
            new Day { Id = 6, DayOfWeek = "Saturday" },
            new Day { Id = 7, DayOfWeek = "Sunday" }
        });
        modelBuilder.Entity<SewClass>().HasData(new SewClass[]
        {
            new SewClass
            {
                Id = 1,
                Name = "Bag class",
                Description = "Learn to build a bag.",
                MaxPeople = 8,
                PricePerPerson = 50.00m,
                Duration = 2
            },
            new SewClass
            {
                Id = 2,
                Name = "Dog Bandana",
                Description = "Learn to build a Dog Bandana.",
                MaxPeople = 6,
                PricePerPerson = 75.00m,
                Duration = 2
            },
            new SewClass
            {
                Id = 3,
                Name = "Mitten Class",
                Description = "Learn to build a Dog Bandana.",
                MaxPeople = 6,
                PricePerPerson = 75.00m,
                Duration = 2
            }
            
        });
        
        modelBuilder.Entity<DayForAvailability>().HasData(new DayForAvailability[]
        {
            new DayForAvailability
            {
                Id = 1,
                DayId = 1, // Monday
            },
            new DayForAvailability
            {
                Id = 2,
                DayId = 3, // Wednesday
            },
            new DayForAvailability
            {
                Id = 3,
                DayId = 5, // Friday
            },
            new DayForAvailability
            {
                Id = 4,
                DayId = 6, // Saturday
            },
            new DayForAvailability
            {
                Id = 5,
                DayId = 7, // Sunday
            }
            
            
        });
        modelBuilder.Entity<Session>().HasData(new Session[]
        {
            new Session
            {
                Id = 1,
                SewClassId = 1, // Sewing Class 1
                DateTime =  DateTime.Parse("2025-02-05"), // Jan 15, 2024, at 10:00 AM
                TimeId = 1, // Example Time reference (10:00 AM)
                DayId = 1, // Example Day reference (Monday)
                Open = true
            },
            new Session
            {
                Id = 2,
                SewClassId = 2, // Sewing Class 2
                DateTime =  DateTime.Parse("2025-02-16"), // June 10, 2024, at 2:00 PM
                TimeId = 2, // Example Time reference (2:00 PM)
                DayId = 2, // Example Day reference (Tuesday)
                Open = true
            },
            new Session
            {
                Id = 3,
                SewClassId = 3, // Sewing Class 3
                DateTime =  DateTime.Parse("2025-02-13"), // April 20, 2024, at 6:00 PM
                TimeId = 3, // Example Time reference (6:00 PM)
                DayId = 5, // Example Day reference (Saturday)
                Open = true
            }
            
        });
        modelBuilder.Entity<Booking>().HasData(new Booking[]
        {
            new Booking
            {
                Id = 1,
                Name = "Carly Olds",
                DateBooked =  DateTime.Parse("2025-02-05"),
                PhoneNumber = "123-456-7890",
                Email = "test@test.com",
                Occupancy = 2,
                SessionId = 1
            },
            new Booking
            {
                Id = 2,
                Name = "Jane Smith",
                DateBooked = DateTime.Parse("2025-02-16"),
                PhoneNumber = "987-654-3210",
                Email = "test2@test.com",
                Occupancy = 3,
                SessionId = 2
            },
            new Booking
            {
                Id = 3,
                Name = "Alice Johnson",
                DateBooked = DateTime.Parse("2025-02-13"),
                PhoneNumber = "555-123-4567",
                Email = "test3@test.com",
                Occupancy = 1,
                SessionId = 3
            }
            
        });

        modelBuilder.Entity<Session>()
            .HasMany(s => s.Employees)
            .WithMany(e => e.Sessions)
            .UsingEntity(j => j.HasData(
                new { SessionsId = 1, EmployeesId = 1 },
                new { SessionsId = 2, EmployeesId = 2 },
                new { SessionsId = 3, EmployeesId = 1 },
                new { SessionsId = 2, EmployeesId = 1 }
                
        ));
        modelBuilder.Entity<DayForAvailability>()
            .HasMany(d => d.Times)
            .WithMany(t => t.DayForAvailabilities)
            .UsingEntity(j => j.HasData(
                // For Session 1 (Day 1: Monday, Time 10:00 AM)
                new { DayForAvailabilitiesId = 1, TimesId = 1 },
                new { DayForAvailabilitiesId = 2, TimesId = 1 }, // Session 1 on Wednesday, Time 10:00 AM

                // For Session 2 (Day 5: Friday, Time 2:00 PM)
                new { DayForAvailabilitiesId = 3, TimesId = 2 },

                // For Session 3 (Day 6: Saturday, Time 6:00 PM)
                new { DayForAvailabilitiesId = 4, TimesId = 3 },
                new { DayForAvailabilitiesId = 5, TimesId = 3 } // Session 3 on Sunday, Time 6:00 PM
        ));

        modelBuilder.Entity<Photo>()
        .HasOne(p => p.SewClass)
        .WithMany(s => s.Photos)
        .HasForeignKey(p => p.SewClassId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Session>()
        .HasOne(sc => sc.SewClass)
        .WithMany(s => s.Sessions)
        .HasForeignKey(sc => sc.SewClassId)
        .OnDelete(DeleteBehavior.Cascade); 


    }
}