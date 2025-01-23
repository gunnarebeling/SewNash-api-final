using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace sewnash_api_jwt.Migrations
{
    /// <inheritdoc />
    public partial class initcreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Days",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DayOfWeek = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Days", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SewClasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    MaxPeople = table.Column<int>(type: "integer", nullable: false),
                    PricePerPerson = table.Column<decimal>(type: "numeric", nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SewClasses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Times",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StartTime = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Times", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DayForAvailabilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DayId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DayForAvailabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DayForAvailabilities_Days_DayId",
                        column: x => x.DayId,
                        principalTable: "Days",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Photos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileKey = table.Column<string>(type: "text", nullable: false),
                    MainPhoto = table.Column<bool>(type: "boolean", nullable: false),
                    SewClassId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Photos_SewClasses_SewClassId",
                        column: x => x.SewClassId,
                        principalTable: "SewClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SewClassId = table.Column<int>(type: "integer", nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TimeId = table.Column<int>(type: "integer", nullable: false),
                    DayId = table.Column<int>(type: "integer", nullable: false),
                    Open = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_Days_DayId",
                        column: x => x.DayId,
                        principalTable: "Days",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sessions_SewClasses_SewClassId",
                        column: x => x.SewClassId,
                        principalTable: "SewClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sessions_Times_TimeId",
                        column: x => x.TimeId,
                        principalTable: "Times",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DayForAvailabilityTime",
                columns: table => new
                {
                    DayForAvailabilitiesId = table.Column<int>(type: "integer", nullable: false),
                    TimesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DayForAvailabilityTime", x => new { x.DayForAvailabilitiesId, x.TimesId });
                    table.ForeignKey(
                        name: "FK_DayForAvailabilityTime_DayForAvailabilities_DayForAvailabil~",
                        column: x => x.DayForAvailabilitiesId,
                        principalTable: "DayForAvailabilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DayForAvailabilityTime_Times_TimesId",
                        column: x => x.TimesId,
                        principalTable: "Times",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DateBooked = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Occupancy = table.Column<int>(type: "integer", nullable: false),
                    SessionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeSession",
                columns: table => new
                {
                    EmployeesId = table.Column<int>(type: "integer", nullable: false),
                    SessionsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeSession", x => new { x.EmployeesId, x.SessionsId });
                    table.ForeignKey(
                        name: "FK_EmployeeSession_Employees_EmployeesId",
                        column: x => x.EmployeesId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeSession_Sessions_SessionsId",
                        column: x => x.SessionsId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Days",
                columns: new[] { "Id", "DayOfWeek" },
                values: new object[,]
                {
                    { 1, "Monday" },
                    { 2, "Tuesday" },
                    { 3, "Wednesday" },
                    { 4, "Thursday" },
                    { 5, "Friday" },
                    { 6, "Saturday" },
                    { 7, "Sunday" }
                });

            migrationBuilder.InsertData(
                table: "SewClasses",
                columns: new[] { "Id", "Description", "Duration", "MaxPeople", "Name", "PricePerPerson" },
                values: new object[,]
                {
                    { 1, "Learn to build a bag.", 2, 8, "Bag class", 50.00m },
                    { 2, "Learn to build a Dog Bandana.", 2, 6, "Dog Bandana", 75.00m },
                    { 3, "Learn to build a Dog Bandana.", 2, 6, "Mitten Class", 75.00m }
                });

            migrationBuilder.InsertData(
                table: "Times",
                columns: new[] { "Id", "StartTime" },
                values: new object[,]
                {
                    { 1, "10:00 AM" },
                    { 2, "11:00 AM" },
                    { 3, "12:00 PM" },
                    { 4, "01:00 PM" },
                    { 5, "02:00 PM" },
                    { 6, "03:00 PM" },
                    { 7, "04:00 PM" },
                    { 8, "05:00 PM" },
                    { 9, "06:00 PM" },
                    { 10, "07:00 PM" },
                    { 11, "08:00 PM" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "PasswordHash", "Role" },
                values: new object[,]
                {
                    { new Guid("d2f2b67f-3a58-4c5e-9dbf-8be4d99b093b"), "JohnDoe@test.com", "$2a$11$85eR9PnVn3MV3JWkEEIhre/H7lsgqe9mIu.gKYFCTOE1sgzX/yWHW", "User" },
                    { new Guid("f5b1f1c6-0e4f-4b0f-8b3b-1b3b8e1b1b1b"), "admina@strator.comx", "$2a$11$LFZXWH/JnSwc9JtUbzNfeOtT427qNasei7gYGDZgCM98GS4GBfQCy", "Admin" }
                });

            migrationBuilder.InsertData(
                table: "DayForAvailabilities",
                columns: new[] { "Id", "DayId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 3 },
                    { 3, 5 },
                    { 4, 6 },
                    { 5, 7 }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Email", "FirstName", "LastName", "PhoneNumber", "UserId" },
                values: new object[,]
                {
                    { 1, "admina@strator.comx", "Admin", "User", "123-123-1234", new Guid("f5b1f1c6-0e4f-4b0f-8b3b-1b3b8e1b1b1b") },
                    { 2, "JohnDoe@test.com", "John", "Doe", "222-222-2222", new Guid("d2f2b67f-3a58-4c5e-9dbf-8be4d99b093b") }
                });

            migrationBuilder.InsertData(
                table: "Sessions",
                columns: new[] { "Id", "DateTime", "DayId", "Open", "SewClassId", "TimeId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, true, 1, 1 },
                    { 2, new DateTime(2025, 2, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, true, 2, 2 },
                    { 3, new DateTime(2025, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, true, 3, 3 }
                });

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "Id", "DateBooked", "Email", "Name", "Occupancy", "PhoneNumber", "SessionId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "test@test.com", "Carly Olds", 2, "123-456-7890", 1 },
                    { 2, new DateTime(2025, 2, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "test2@test.com", "Jane Smith", 3, "987-654-3210", 2 },
                    { 3, new DateTime(2025, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "test3@test.com", "Alice Johnson", 1, "555-123-4567", 3 }
                });

            migrationBuilder.InsertData(
                table: "DayForAvailabilityTime",
                columns: new[] { "DayForAvailabilitiesId", "TimesId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 3, 2 },
                    { 4, 3 },
                    { 5, 3 }
                });

            migrationBuilder.InsertData(
                table: "EmployeeSession",
                columns: new[] { "EmployeesId", "SessionsId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 1, 3 },
                    { 2, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_SessionId",
                table: "Bookings",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_DayForAvailabilities_DayId",
                table: "DayForAvailabilities",
                column: "DayId");

            migrationBuilder.CreateIndex(
                name: "IX_DayForAvailabilityTime_TimesId",
                table: "DayForAvailabilityTime",
                column: "TimesId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_UserId",
                table: "Employees",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSession_SessionsId",
                table: "EmployeeSession",
                column: "SessionsId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_SewClassId",
                table: "Photos",
                column: "SewClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_DayId",
                table: "Sessions",
                column: "DayId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_SewClassId",
                table: "Sessions",
                column: "SewClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_TimeId",
                table: "Sessions",
                column: "TimeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "DayForAvailabilityTime");

            migrationBuilder.DropTable(
                name: "EmployeeSession");

            migrationBuilder.DropTable(
                name: "Photos");

            migrationBuilder.DropTable(
                name: "DayForAvailabilities");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Days");

            migrationBuilder.DropTable(
                name: "SewClasses");

            migrationBuilder.DropTable(
                name: "Times");
        }
    }
}
