using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Climb.Services
{
    public class S3Cdn : CdnService
    {
        private readonly AmazonS3Client s3Client;
        private readonly string accessKey;
        private readonly string secretKey;
        private readonly string bucketName;
        private readonly string environment;

        public S3Cdn(IConfiguration configuration)
        {
            var awsSection = configuration.GetSection("AWS");

            accessKey = awsSection["AccessKey"];
            secretKey = awsSection["SecretKey"];
            bucketName = awsSection["Bucket"];
            environment = awsSection["Environment"];
            var region = RegionEndpoint.GetBySystemName(awsSection["Region"]);

            root = $"https://s3.amazonaws.com/{bucketName}/{environment}";

            var credentials = new BasicAWSCredentials(accessKey, secretKey);
            s3Client = new AmazonS3Client(credentials, region);
        }

        protected override async Task UploadImageInternalAsync(IFormFile image, string folder, string fileKey)
        {
            var transfer = new TransferUtility(accessKey, secretKey, RegionEndpoint.USEast1);
            await transfer.UploadAsync(image.OpenReadStream(), string.Join("/", bucketName, environment, folder), fileKey);
        }

        protected override void EnsureFolder(string rulesFolder)
        {
            // Paths are created when uploading.
        }

        public override async Task DeleteImageAsync(string fileKey, ImageRules rules)
        {
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = $"{environment}/{rules.Folder}/{fileKey}"
            };
            await s3Client.DeleteObjectAsync(deleteRequest);
        }
    }
}