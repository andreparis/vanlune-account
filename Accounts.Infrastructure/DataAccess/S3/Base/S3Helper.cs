using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Accounts.Domain.DataAccess.S3.Base;
using Accounts.Infraestructure.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Accounts.Infraestructure.DataAccess.S3
{
    public class S3Helper<T> : IS3Helper<T> where T : class
    {
        private readonly IAmazonS3 _amazonS3;
        private readonly ILogger _logger;
        private readonly string _bucketName;


        public S3Helper(ILogger logger,
            IConfiguration configuration)
        {
            _logger = logger;

            if (Debugger.IsAttached)
            {
                _amazonS3 = new AmazonS3Client(new BasicAWSCredentials(configuration["AWS:AccessKey"], configuration["AWS:SecretKey"]), RegionEndpoint.USEast1);
                _bucketName = configuration["AWS:S3:Bucket"];
            }
            else
            {
                _amazonS3 = new AmazonS3Client(RegionEndpoint.USEast1);
                _bucketName = configuration["AWS_S3_Bucket"];
            }            
        }

        public IEnumerable<T> GetList(string keyName)
        {
            _logger.Info($"Getting file at s3 bucketName -> {_bucketName}; key -> {keyName}");
            try
            {
                if (ListObjectsInS3(keyName).S3Objects.Count == 0)
                {
                    return null;
                }

                var getRequest = new GetObjectRequest
                {
                    BucketName = _bucketName,
                    Key = keyName,
                };

                var response = _amazonS3.GetObjectAsync(getRequest)?.Result;
                StreamReader reader = new StreamReader(response.ResponseStream);

                var jsonParse = JsonConvert.DeserializeObject<IEnumerable<T>>(reader.ReadToEnd());

                _logger.Info($"File retrieved from s3 -> {_bucketName}");
                _logger.Info(JsonConvert.SerializeObject(jsonParse, Formatting.Indented));

                return jsonParse;
            }
            catch (AmazonS3Exception e)
            {
                _logger.Info($"{e.Message}");
                throw;
            }
            catch (System.Exception ex)
            {
                _logger.Info(ex.InnerException.ToString());
                _logger.Info(ex.StackTrace);
                throw;
            }
        }

        public T GetJsonFile(string keyName)
        {
            _logger.Info($"Getting file at s3 bucketName -> {_bucketName}; key -> {keyName}");
            try
            {
                if (ListObjectsInS3(keyName).S3Objects.Count == 0)
                {
                    return null;
                }

                var getRequest = new GetObjectRequest
                {
                    BucketName = _bucketName,
                    Key = keyName,
                };

                var response = _amazonS3.GetObjectAsync(getRequest)?.Result;
                StreamReader reader = new StreamReader(response.ResponseStream);

                var jsonParse = JsonConvert.DeserializeObject<T>(reader.ReadToEnd());

                _logger.Info($"File retrieved from s3 -> {_bucketName}");
                _logger.Info(JsonConvert.SerializeObject(jsonParse, Formatting.Indented));

                return jsonParse;
            }
            catch (AmazonS3Exception e)
            {
                _logger.Info($"{e.Message}");
                throw;
            }
            catch (System.Exception ex)
            {
                _logger.Info(ex.InnerException.ToString());
                _logger.Info(ex.StackTrace);
                throw;
            }
        }

        public string GetAsString(string keyName)
        {
            _logger.Info($"Getting file at s3 bucketName -> {_bucketName}; key -> {keyName}");
            try
            {
                if (ListObjectsInS3(keyName).S3Objects.Count == 0)
                {
                    return null;
                }

                var getRequest = new GetObjectRequest
                {
                    BucketName = _bucketName,
                    Key = keyName,
                };

                var response = _amazonS3.GetObjectAsync(getRequest)?.Result;
                StreamReader reader = new StreamReader(response.ResponseStream);

                return reader.ReadToEnd();
            }
            catch (AmazonS3Exception e)
            {
                _logger.Info($"{e.Message}");
                throw;
            }
            catch (System.Exception ex)
            {
                _logger.Info(ex.InnerException.ToString());
                _logger.Info(ex.StackTrace);
                throw;
            }
        }

        public IEnumerable<T> GetAll(string keyName)
        {

            var s3Objects = ListObjectsInS3(keyName);
            var objects = new List<T>();

            foreach (var s3Object in s3Objects.S3Objects)
            {
                var getRequest = new GetObjectRequest
                {
                    BucketName = _bucketName,
                    Key = s3Object.Key,
                };

                var response = _amazonS3.GetObjectAsync(getRequest)?.Result;
                StreamReader reader = new StreamReader(response.ResponseStream);

                objects.Add(JsonConvert.DeserializeObject<T>(reader.ReadToEnd()));
            }

            return objects;
        }

        public void Upload(string message, string keyName, string contentType = "application/json")
        {
            _logger.Info($"Saving file at s3 bucketName -> {_bucketName}; key -> {keyName}");
            _logger.Info(JsonConvert.SerializeObject(JsonConvert.DeserializeObject(message), Formatting.Indented));
            try
            {
                var putRequest = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = keyName,
                    ContentBody = message,
                    ContentType = contentType,
                    ServerSideEncryptionMethod = ServerSideEncryptionMethod.None
                };

                var response = _amazonS3.PutObjectAsync(putRequest).Result;

                _logger.Info($"File saved at s3 -> {_bucketName}");
            }
            catch (AmazonS3Exception e)
            {
                _logger.Info($"{e.Message}");
                throw;
            }
            catch (System.Exception ex)
            {
                _logger.Info(ex.InnerException.ToString());
                _logger.Info(ex.StackTrace);
                throw;
            }
        }

        public void Delete(string keyName)
        {
            _logger.Info($"Removing file from s3 _bucketName -> {_bucketName}; key -> {keyName}");

            try
            {
                var deleteRequest = new DeleteObjectRequest()
                {
                    BucketName = _bucketName,
                    Key = keyName
                };

                var response = _amazonS3.DeleteObjectAsync(deleteRequest).Result;

                _logger.Info($"File removed from s3 -> {_bucketName}");
            }
            catch (AmazonS3Exception e)
            {
                _logger.Info($"{e.Message}");
                throw;
            }
            catch (System.Exception ex)
            {
                _logger.Info(ex.InnerException.ToString());
                _logger.Info(ex.StackTrace);
                throw;
            }
        }

        public ListObjectsV2Response ListObjectsInS3(string keyName) => _amazonS3
                .ListObjectsV2Async(new ListObjectsV2Request
                {
                    BucketName = _bucketName,
                    Prefix = keyName,
                }).Result;
    }
}
