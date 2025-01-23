using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using SewNash.Models.DTOs;

namespace SewNash.Controllers
{
    public class PhotoParent : ControllerBase
    {
        protected IConfiguration _configuration;
        protected readonly IAmazonS3 _s3Client;
            
        private const string BucketName = "sewnashbucket";

        public PhotoParent( IAmazonS3 s3Client)
        {
            
           _s3Client = s3Client;
        }

        protected ClassWithPhotos ConvertFileKey( ClassWithPhotos sewClass)
        {
            
                sewClass.Photos.ForEach(p => p.FileKey = _s3Client.GetPreSignedURL(new GetPreSignedUrlRequest
                {
                    BucketName = BucketName,
                    Key = p.FileKey,
                    Expires = DateTime.Now.AddMinutes(5)
                }));
                
            
            return sewClass;
        }
        
        
    }
}