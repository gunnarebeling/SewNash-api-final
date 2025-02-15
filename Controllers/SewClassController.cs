using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SewNash.Data;
using Microsoft.EntityFrameworkCore;
using SewNash.Models;
using SewNash.Models.DTOs;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Connections;
using StackExchange.Redis;
using NRedisStack.RedisStackCommands;
using NetTopologySuite.IO;
using System.Threading.Tasks;
using NRedisStack.Search;
using NRedisStack.Search.Literals.Enums;

namespace SewNash.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SewClassController : PhotoParent
{
    private SewNashDbContext _dbContext;
    private IMapper _mapper;
    private readonly IConnectionMultiplexer _redis;
    private ILogger<SewClassController> _logger;
   
    private const string BucketName = "sewnashbucket";

    public SewClassController(SewNashDbContext context, IMapper mapper, IAmazonS3 s3Client, IConnectionMultiplexer redis, ILogger<SewClassController> logger)
            : base(s3Client) // Pass the dependencies to the PhotoParent constructor
        {
            _dbContext = context;
            _mapper = mapper;
            _redis = redis;
            _logger = logger;
        }

    [HttpGet]
    public IActionResult Get()
    {

        List<SewClassDTO> sewClasses = _dbContext.SewClasses.ProjectTo<SewClassDTO>(_mapper.ConfigurationProvider).ToList();
        sewClasses.ForEach(c => ConvertFileKey(c) );
        return Ok(sewClasses);
    }

    [HttpGet("{id}")]
    public IActionResult GetDetails(string id)
    {
        SewClassDTO sewClass = _dbContext.SewClasses.ProjectTo<SewClassDTO>(_mapper.ConfigurationProvider).Single(s => s.Id == int.Parse(id));
        return Ok(sewClass);
    }
    [HttpPost]
    [Authorize]
    public IActionResult Post([FromBody] PostClassDTO sewClass)
    {
        SewClass newClass = _mapper.Map<SewClass>(sewClass);
        
        _dbContext.SewClasses.Add(newClass);
        _dbContext.SaveChanges();
        return Created($"/api/sewClass/{newClass.Id}", newClass);
        
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(string id)
    {
        var redis = _redis.GetDatabase();
        var redisdb = redis.JSON();

        SewClass sewClass = _dbContext.SewClasses.SingleOrDefault(c => c.Id == int.Parse(id));
        if (sewClass == default)
        {
            return NotFound();
        }

        bool hasBookings = _dbContext.Bookings
        .Include(b => b.Session) // Eager load Session
        .Any(b => b.Session.SewClassId == sewClass.Id && b.Session.DateTime >= DateTime.Now);
        if (hasBookings)
        {
            return BadRequest("there are bookings for this class");
        }
       // Create an index on the JSON documents
        var schema = new Schema()
            .AddNumericField(new FieldName("$.SewClassId", "SewClassId"));

        // Check if the index already exists
        var server = _redis.GetServer(_redis.GetEndPoints().First());
        var indexExists = server.Keys(pattern: "idx:sessions").Any();
        if (!indexExists)
        {
            bool indexCreated = redis.FT().Create(
                "idx:sessions",
                new FTCreateParams()
                    .On(IndexDataType.JSON)
                    .Prefix("session:"),
                schema
            );
            _logger.LogInformation($"Index created: {indexCreated}");
        }

        // Query the index to find sessions with the specified SewClassId
        int offset = 0;
        int limit = 1000; // Adjust the limit as needed
        var query = new Query($"@SewClassId:[{id} {id}]").Limit(offset, limit);
        var searchResult = redis.FT().Search("idx:sessions", query);

        _logger.LogInformation($"Search result count: {searchResult.TotalResults}");

        while (searchResult.Documents.Count > 0)
        {
            _logger.LogInformation($"Search result count: {searchResult.TotalResults}");

            foreach (var doc in searchResult.Documents)
            {
                var key = doc.Id;
                redis.KeyDelete(key);
                _logger.LogWarning($"Deleted session: {key}");
            }

            offset += limit;
            query = new Query($"@SewClassId:[{id} {id}]").Limit(offset, limit);
            searchResult = redis.FT().Search("idx:sessions", query);
        }
        _dbContext.Remove(sewClass);
        _dbContext.SaveChanges();
        return NoContent();
    }

    [HttpPut("{id}")]
    [Authorize]
    public IActionResult Update(string id, [FromBody] PostClassDTO classData)
    {
        SewClass sewClass = _dbContext.SewClasses.SingleOrDefault(c => c.Id == int.Parse(id));
        if (sewClass == default)
        {
            return NotFound();
        }
        sewClass.Name = classData.Name;
        sewClass.Description = classData.Description;
        sewClass.Duration = classData.Duration;
        sewClass.MaxPeople = classData.MaxPeople;
        sewClass.PricePerPerson = classData.PricePerPerson;
        _dbContext.SaveChanges();
        return NoContent();
    }
    

}