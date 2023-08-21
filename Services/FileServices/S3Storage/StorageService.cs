using Amazon.S3;
using Amazon.S3.Model;
using Common.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Services.S3Storage
{
    public class StorageService : IS3StorageService
    {
        private readonly string _bucketName;
        private readonly AmazonS3Client _client;
        private readonly ILogger<IS3StorageService> logger;
        private readonly IConfiguration _configuration;


        public StorageService(IConfiguration configuration, ILogger<IS3StorageService> _logger)
        {
            _bucketName = configuration["S3:BucketName"];
            logger = _logger;
            _configuration = configuration;
            _client = new AmazonS3Client(
                configuration["S3:AccessKeyId"],
                configuration["S3:SecretAccessKey"],
                new AmazonS3Config
                {
                    ServiceURL = configuration["S3:ServiceEndpoint"],
                    ForcePathStyle = true
                });
        }

        public async Task<string> UploadToS3Async(string key, Stream stream)
        {
           var result = await _client.PutObjectAsync(new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = key,
                InputStream = stream,
                AutoCloseStream = true,
                ContentType = "application/pdf"
            });

            if (result.HttpStatusCode != HttpStatusCode.OK)
            {
                logger.LogError($"UploadToS3Async - error while calling UploadToS3Async Endpoint");
                throw new CustomHttpException(result.HttpStatusCode, "UploadToS3Async - Error_Occured");
            }

            var url = _client.GetPreSignedURL(new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = key,
                Expires = DateTime.Now.AddDays(385),
            });

            return url;
        }

        public async Task<Stream> GetObjectAsync(string key)
        {
            var response = await _client.GetObjectAsync(new GetObjectRequest
            {
                Key = key,
                BucketName = _bucketName
            });

            return response.ResponseStream;
        }

        public async Task<string> GetUrlAsync(string key)
        {
            var url = _client.GetPreSignedURL(new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = key,
                Expires = DateTime.Now.AddDays(385),
            });

            return url;
        }

        public async Task<bool> DeleteObjectAsync(string key)
        {
            var result = await _client.DeleteObjectAsync(new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = key
            });

            return result.HttpStatusCode == HttpStatusCode.OK;
        }

    }
}
