using System.Text.Json.Serialization;
using SewNash.Data;
using Amazon.S3;
using dotenv.net;
using Stripe;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon;
using Microsoft.EntityFrameworkCore;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

DotEnv.Load();
// Add services to the container.
var seqKey = Environment.GetEnvironmentVariable("SEQ_KEY");
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341", apiKey: seqKey) // Replace with your Seq server URL
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers().AddJsonOptions(opts =>
{
    opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

string adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD")!;
string JohnDoePassword = Environment.GetEnvironmentVariable("JOHNDOE_PASSWORD")!;
string adminPasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword);
string JohnDoePasswordHash = BCrypt.Net.BCrypt.HashPassword(JohnDoePassword);
builder.Configuration["AdminPassword"] = adminPasswordHash;
builder.Configuration["JohnDoePassword"] = JohnDoePasswordHash;

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug(); 
builder.Logging.AddSerilog();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowS3App",
                policy => policy
                    .AllowAnyOrigin() 
                    .AllowAnyHeader()
                    .AllowAnyMethod());
        });

builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets<Program>(); // Load User Secrets
var awsOptions = new AWSOptions
{
    Credentials = new BasicAWSCredentials(
        Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID"),
        Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY")
    ),
    Region = RegionEndpoint.GetBySystemName(Environment.GetEnvironmentVariable("AWS_REGION"))
};
// Optionally, bind AWS options from configuration
builder.Services.AddDefaultAWSOptions(awsOptions);
builder.Services.AddAWSService<IAmazonS3>();


AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);



var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SewNashAPI", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token like this: Bearer {your token}"
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            securityScheme, new[] { "Bearer" }
        }
    };

    c.AddSecurityRequirement(securityRequirement);
});


// allows passing datetimes without time zone data 
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// allows our api endpoints to access the database through Entity Framework Core
builder.Services.AddNpgsql<SewNashDbContext>(Environment.GetEnvironmentVariable("CONNECTION_STRING"));
StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("STRIPE_SECRET");
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<SewNashDbContext>();
        context.Database.Migrate();
       
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SewNashAPI v1");
        c.RoutePrefix = string.Empty;
    });
}
app.UseCors("AllowS3App");
app.UseHttpsRedirection();
// these two calls are required to add auth to the pipeline for a request
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();