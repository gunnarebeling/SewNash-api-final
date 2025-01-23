﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SewNash.Data;

#nullable disable

namespace sewnash_api_jwt.Migrations
{
    [DbContext(typeof(SewNashDbContext))]
    [Migration("20250122230040_initcreate")]
    partial class initcreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DayForAvailabilityTime", b =>
                {
                    b.Property<int>("DayForAvailabilitiesId")
                        .HasColumnType("integer");

                    b.Property<int>("TimesId")
                        .HasColumnType("integer");

                    b.HasKey("DayForAvailabilitiesId", "TimesId");

                    b.HasIndex("TimesId");

                    b.ToTable("DayForAvailabilityTime");

                    b.HasData(
                        new
                        {
                            DayForAvailabilitiesId = 1,
                            TimesId = 1
                        },
                        new
                        {
                            DayForAvailabilitiesId = 2,
                            TimesId = 1
                        },
                        new
                        {
                            DayForAvailabilitiesId = 3,
                            TimesId = 2
                        },
                        new
                        {
                            DayForAvailabilitiesId = 4,
                            TimesId = 3
                        },
                        new
                        {
                            DayForAvailabilitiesId = 5,
                            TimesId = 3
                        });
                });

            modelBuilder.Entity("EmployeeSession", b =>
                {
                    b.Property<int>("EmployeesId")
                        .HasColumnType("integer");

                    b.Property<int>("SessionsId")
                        .HasColumnType("integer");

                    b.HasKey("EmployeesId", "SessionsId");

                    b.HasIndex("SessionsId");

                    b.ToTable("EmployeeSession");

                    b.HasData(
                        new
                        {
                            EmployeesId = 1,
                            SessionsId = 1
                        },
                        new
                        {
                            EmployeesId = 2,
                            SessionsId = 2
                        },
                        new
                        {
                            EmployeesId = 1,
                            SessionsId = 3
                        },
                        new
                        {
                            EmployeesId = 1,
                            SessionsId = 2
                        });
                });

            modelBuilder.Entity("SewNash.Models.Booking", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DateBooked")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Occupancy")
                        .HasColumnType("integer");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("SessionId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("SessionId");

                    b.ToTable("Bookings");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            DateBooked = new DateTime(2025, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "test@test.com",
                            Name = "Carly Olds",
                            Occupancy = 2,
                            PhoneNumber = "123-456-7890",
                            SessionId = 1
                        },
                        new
                        {
                            Id = 2,
                            DateBooked = new DateTime(2025, 2, 16, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "test2@test.com",
                            Name = "Jane Smith",
                            Occupancy = 3,
                            PhoneNumber = "987-654-3210",
                            SessionId = 2
                        },
                        new
                        {
                            Id = 3,
                            DateBooked = new DateTime(2025, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "test3@test.com",
                            Name = "Alice Johnson",
                            Occupancy = 1,
                            PhoneNumber = "555-123-4567",
                            SessionId = 3
                        });
                });

            modelBuilder.Entity("SewNash.Models.Day", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("DayOfWeek")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Days");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            DayOfWeek = "Monday"
                        },
                        new
                        {
                            Id = 2,
                            DayOfWeek = "Tuesday"
                        },
                        new
                        {
                            Id = 3,
                            DayOfWeek = "Wednesday"
                        },
                        new
                        {
                            Id = 4,
                            DayOfWeek = "Thursday"
                        },
                        new
                        {
                            Id = 5,
                            DayOfWeek = "Friday"
                        },
                        new
                        {
                            Id = 6,
                            DayOfWeek = "Saturday"
                        },
                        new
                        {
                            Id = 7,
                            DayOfWeek = "Sunday"
                        });
                });

            modelBuilder.Entity("SewNash.Models.DayForAvailability", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("DayId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("DayId");

                    b.ToTable("DayForAvailabilities");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            DayId = 1
                        },
                        new
                        {
                            Id = 2,
                            DayId = 3
                        },
                        new
                        {
                            Id = 3,
                            DayId = 5
                        },
                        new
                        {
                            Id = 4,
                            DayId = 6
                        },
                        new
                        {
                            Id = 5,
                            DayId = 7
                        });
                });

            modelBuilder.Entity("SewNash.Models.Employee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Employees");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Email = "admina@strator.comx",
                            FirstName = "Admin",
                            LastName = "User",
                            PhoneNumber = "123-123-1234",
                            UserId = new Guid("f5b1f1c6-0e4f-4b0f-8b3b-1b3b8e1b1b1b")
                        },
                        new
                        {
                            Id = 2,
                            Email = "JohnDoe@test.com",
                            FirstName = "John",
                            LastName = "Doe",
                            PhoneNumber = "222-222-2222",
                            UserId = new Guid("d2f2b67f-3a58-4c5e-9dbf-8be4d99b093b")
                        });
                });

            modelBuilder.Entity("SewNash.Models.Photo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("FileKey")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("MainPhoto")
                        .HasColumnType("boolean");

                    b.Property<int?>("SewClassId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("SewClassId");

                    b.ToTable("Photos");
                });

            modelBuilder.Entity("SewNash.Models.Session", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("DayId")
                        .HasColumnType("integer");

                    b.Property<bool>("Open")
                        .HasColumnType("boolean");

                    b.Property<int>("SewClassId")
                        .HasColumnType("integer");

                    b.Property<int>("TimeId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("DayId");

                    b.HasIndex("SewClassId");

                    b.HasIndex("TimeId");

                    b.ToTable("Sessions");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            DateTime = new DateTime(2025, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            DayId = 1,
                            Open = true,
                            SewClassId = 1,
                            TimeId = 1
                        },
                        new
                        {
                            Id = 2,
                            DateTime = new DateTime(2025, 2, 16, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            DayId = 2,
                            Open = true,
                            SewClassId = 2,
                            TimeId = 2
                        },
                        new
                        {
                            Id = 3,
                            DateTime = new DateTime(2025, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            DayId = 5,
                            Open = true,
                            SewClassId = 3,
                            TimeId = 3
                        });
                });

            modelBuilder.Entity("SewNash.Models.SewClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Duration")
                        .HasColumnType("integer");

                    b.Property<int>("MaxPeople")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("PricePerPerson")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.ToTable("SewClasses");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "Learn to build a bag.",
                            Duration = 2,
                            MaxPeople = 8,
                            Name = "Bag class",
                            PricePerPerson = 50.00m
                        },
                        new
                        {
                            Id = 2,
                            Description = "Learn to build a Dog Bandana.",
                            Duration = 2,
                            MaxPeople = 6,
                            Name = "Dog Bandana",
                            PricePerPerson = 75.00m
                        },
                        new
                        {
                            Id = 3,
                            Description = "Learn to build a Dog Bandana.",
                            Duration = 2,
                            MaxPeople = 6,
                            Name = "Mitten Class",
                            PricePerPerson = 75.00m
                        });
                });

            modelBuilder.Entity("SewNash.Models.Time", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("StartTime")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Times");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            StartTime = "10:00 AM"
                        },
                        new
                        {
                            Id = 2,
                            StartTime = "11:00 AM"
                        },
                        new
                        {
                            Id = 3,
                            StartTime = "12:00 PM"
                        },
                        new
                        {
                            Id = 4,
                            StartTime = "01:00 PM"
                        },
                        new
                        {
                            Id = 5,
                            StartTime = "02:00 PM"
                        },
                        new
                        {
                            Id = 6,
                            StartTime = "03:00 PM"
                        },
                        new
                        {
                            Id = 7,
                            StartTime = "04:00 PM"
                        },
                        new
                        {
                            Id = 8,
                            StartTime = "05:00 PM"
                        },
                        new
                        {
                            Id = 9,
                            StartTime = "06:00 PM"
                        },
                        new
                        {
                            Id = 10,
                            StartTime = "07:00 PM"
                        },
                        new
                        {
                            Id = 11,
                            StartTime = "08:00 PM"
                        });
                });

            modelBuilder.Entity("SewNash.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = new Guid("f5b1f1c6-0e4f-4b0f-8b3b-1b3b8e1b1b1b"),
                            Email = "admina@strator.comx",
                            PasswordHash = "$2a$11$LFZXWH/JnSwc9JtUbzNfeOtT427qNasei7gYGDZgCM98GS4GBfQCy",
                            Role = "Admin"
                        },
                        new
                        {
                            Id = new Guid("d2f2b67f-3a58-4c5e-9dbf-8be4d99b093b"),
                            Email = "JohnDoe@test.com",
                            PasswordHash = "$2a$11$85eR9PnVn3MV3JWkEEIhre/H7lsgqe9mIu.gKYFCTOE1sgzX/yWHW",
                            Role = "User"
                        });
                });

            modelBuilder.Entity("DayForAvailabilityTime", b =>
                {
                    b.HasOne("SewNash.Models.DayForAvailability", null)
                        .WithMany()
                        .HasForeignKey("DayForAvailabilitiesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SewNash.Models.Time", null)
                        .WithMany()
                        .HasForeignKey("TimesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("EmployeeSession", b =>
                {
                    b.HasOne("SewNash.Models.Employee", null)
                        .WithMany()
                        .HasForeignKey("EmployeesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SewNash.Models.Session", null)
                        .WithMany()
                        .HasForeignKey("SessionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SewNash.Models.Booking", b =>
                {
                    b.HasOne("SewNash.Models.Session", "Session")
                        .WithMany("Bookings")
                        .HasForeignKey("SessionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Session");
                });

            modelBuilder.Entity("SewNash.Models.DayForAvailability", b =>
                {
                    b.HasOne("SewNash.Models.Day", "Day")
                        .WithMany()
                        .HasForeignKey("DayId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Day");
                });

            modelBuilder.Entity("SewNash.Models.Employee", b =>
                {
                    b.HasOne("SewNash.Models.User", "User")
                        .WithOne("Employee")
                        .HasForeignKey("SewNash.Models.Employee", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("SewNash.Models.Photo", b =>
                {
                    b.HasOne("SewNash.Models.SewClass", "SewClass")
                        .WithMany("Photos")
                        .HasForeignKey("SewClassId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("SewClass");
                });

            modelBuilder.Entity("SewNash.Models.Session", b =>
                {
                    b.HasOne("SewNash.Models.Day", "Day")
                        .WithMany()
                        .HasForeignKey("DayId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SewNash.Models.SewClass", "SewClass")
                        .WithMany("Sessions")
                        .HasForeignKey("SewClassId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SewNash.Models.Time", "Time")
                        .WithMany()
                        .HasForeignKey("TimeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Day");

                    b.Navigation("SewClass");

                    b.Navigation("Time");
                });

            modelBuilder.Entity("SewNash.Models.Session", b =>
                {
                    b.Navigation("Bookings");
                });

            modelBuilder.Entity("SewNash.Models.SewClass", b =>
                {
                    b.Navigation("Photos");

                    b.Navigation("Sessions");
                });

            modelBuilder.Entity("SewNash.Models.User", b =>
                {
                    b.Navigation("Employee")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
