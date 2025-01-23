using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SewNash.Data;
using SewNash.Models;
using SewNash.Models.DTOs;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.VisualBasic;
using Microsoft.Extensions.Logging;


namespace SewNash.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PhotoController : ControllerBase
{
    private SewNashDbContext _dbContext;
    private IMapper _mapper;
    private IConfiguration _configuration;
    private readonly IAmazonS3 _s3Client;
    private readonly ILogger<PhotoController> _logger;
    
    private const string BucketName = "sewnashbucket";

    public PhotoController(SewNashDbContext context, IMapper mapper, IConfiguration configuration, IAmazonS3 s3Client, ILogger<PhotoController> logger)
    {
        _dbContext = context;
        _mapper = mapper;
        _configuration= configuration;
        _s3Client = s3Client;
        _logger = logger;
        _s3Client = new AmazonS3Client(
            Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID"),
            Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY"),
            new AmazonS3Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.USEast2 // Correct region for your bucket
            });
            }


    [HttpPost("upload")]
    [Consumes("multipart/form-data")] 
    public async Task<IActionResult> UploadPhoto([FromForm] IFormFile file, [FromQuery] int? classId)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is required");
        }
        string fileKey;
        if (classId.HasValue)
        {
            fileKey = $"sewclass_{classId.Value}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        }
        else
        {
            fileKey = $"no_class_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        }
        using (var stream = file.OpenReadStream())
        {
            var putRequest = new PutObjectRequest
            {
                BucketName = BucketName,
                Key = fileKey,
                InputStream = stream,
                ContentType = file.ContentType
                
            };

            try
            {
                var response = await _s3Client.PutObjectAsync(putRequest);
               
                Photo photo = new Photo
                {
                    FileKey = fileKey,
                    SewClassId = classId,
                    
                };
                if (!_dbContext.Photos.Any(p => p.SewClassId == classId))
                {
                    photo.MainPhoto = true;
                }
                _dbContext.Photos.AddAsync(photo);
            
                await _dbContext.SaveChangesAsync();

                return Ok(new { FileUrl = $"https://{BucketName}.s3.amazonaws.com/{fileKey}"});
            }
            catch (AmazonS3Exception s3Ex)
            {
                _logger.LogError(s3Ex, "Error uploading file to S3");
                return StatusCode(500, $"Internal server error: {s3Ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal server error");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

    // GET function to retrieve the photo's URL from S3
    [HttpGet("class/{classId}")]
public async Task<IActionResult> GetPhoto(int classId)
{
    if (classId <= 0)
    {
        return BadRequest("Class ID is required");
    }

    List<PhotoDTO> photos = _dbContext.Photos
        .Where(p => p.SewClassId == classId)
        .ProjectTo<PhotoDTO>(_mapper.ConfigurationProvider).ToList();
        

    try
    {
       foreach (var photo in photos)
                {
                    var request = new GetPreSignedUrlRequest
                    {
                        BucketName = BucketName,
                        Key = photo.FileKey,
                        Expires = DateTime.Now.AddHours(1)
                    };

                    photo.FileKey = _s3Client.GetPreSignedURL(request);
                }

                return Ok(photos);
    }
    catch (AmazonS3Exception s3Ex)
    {
        // Handle S3 specific exceptions
        return NotFound($"File not found: {s3Ex.Message}");
    }
    catch (Exception ex)
    {
        // Handle general exceptions
        return StatusCode(500, $"Internal server error: {ex.Message}");
    }
}

    // DELETE function to remove the photo from S3
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePhoto( int id)
    {
        
        Photo photoObj = _dbContext.Photos.SingleOrDefault(p => p.Id == id);
        if (photoObj == default)
        {
            return NotFound("photo not found");
        }

        var deleteRequest = new DeleteObjectRequest
        {
            BucketName = BucketName,
            Key = photoObj.FileKey
        };

        try
        {
            var response = await _s3Client.DeleteObjectAsync(deleteRequest);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.NoContent)
            {
                _dbContext.Photos.Remove(photoObj);
                await _dbContext.SaveChangesAsync();
                return Ok(new { Message = "File deleted successfully" });
            }
            else
            {
                return NotFound("File not found in S3");
            }
        }
        catch (AmazonS3Exception s3Ex)
        {
            return StatusCode(500, $"Failed to delete file: {s3Ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPut("class/{classId}/setmain/{photoId}")]
    [Authorize]
    public IActionResult UpdateMain(int classId, int photoId)
    {
        Photo photo = _dbContext.Photos.SingleOrDefault(p => p.Id == photoId);
        Photo oldMain = _dbContext.Photos.SingleOrDefault(p => p.MainPhoto && p.SewClassId == classId);
        if (photo == default)
        {
            return NotFound("photo not found");

        }
        photo.MainPhoto = true;
        if (oldMain != default && oldMain.Id != photoId)
        {
            oldMain.MainPhoto = false;
            
        }
        _dbContext.SaveChanges();
        return NoContent();

    }
}