using Amazon;
using Amazon.Runtime;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Accounts.Infraestructure.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Policy;
using System.Text;

namespace Accounts.Infraestructure.Security
{
    public class AwsSecretManagerService : IAwsSecretManagerService
    {
        private readonly AmazonSecretsManagerClient _secretsManagerClient;
        private readonly ILogger _logger;
        private static Dictionary<string, string> _dictionary = new Dictionary<string, string>();

        public AwsSecretManagerService(IConfiguration configuration,
            ILogger logger)
        {
            _logger = logger;

            if (Debugger.IsAttached)
            {
                _secretsManagerClient = new AmazonSecretsManagerClient(new BasicAWSCredentials(configuration["AWS:AccessKey"], configuration["AWS:SecretKey"]), RegionEndpoint.USEast1);
            }
            else
            {
                _logger.Info($"Lambda region is {configuration["AWS_REGION"]}");

                _secretsManagerClient = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(configuration["AWS_REGION"]));
            }   
        }

        public string GetSecret(string secret)
        {
            if (!string.IsNullOrEmpty(secret) && _dictionary.ContainsKey(secret))
            {
                _logger.Info("Key already recovered, returning conection string");
                return _dictionary[secret];
            }

            var memoryStream = new MemoryStream();

            _logger.Info($"Lambda secretName is {secret}");

            GetSecretValueRequest request = new GetSecretValueRequest
            {
                SecretId = secret,
                VersionStage = "AWSCURRENT" // VersionStage defaults to AWSCURRENT if unspecified.
            };

            GetSecretValueResponse response = null;

            // In this sample we only handle the specific exceptions for the 'GetSecretValue' API.
            // See https://docs.aws.amazon.com/secretsmanager/latest/apireference/API_GetSecretValue.html
            // We rethrow the exception by default.

            try
            {
                response = _secretsManagerClient.GetSecretValueAsync(request).Result;
            }
            catch (DecryptionFailureException e)
            {
                _logger.Error($"{e.Message} at {e.StackTrace}");
                // Secrets Manager can't decrypt the protected secret text using the provided KMS key.
                // Deal with the exception here, and/or rethrow at your discretion.
                throw;
            }
            catch (InternalServiceErrorException e)
            {
                _logger.Error($"{e.Message} at {e.StackTrace}");
                // An error occurred on the server side.
                // Deal with the exception here, and/or rethrow at your discretion.
                throw;
            }
            catch (InvalidParameterException e)
            {
                _logger.Error($"{e.Message} at {e.StackTrace}");
                // You provided an invalid value for a parameter.
                // Deal with the exception here, and/or rethrow at your discretion
                throw;
            }
            catch (InvalidRequestException e)
            {
                _logger.Error($"{e.Message} at {e.StackTrace}");
                // You provided a parameter value that is not valid for the current state of the resource.
                // Deal with the exception here, and/or rethrow at your discretion.
                throw;
            }
            catch (ResourceNotFoundException e)
            {
                _logger.Error($"{e.Message} at {e.StackTrace}");
                // We can't find the resource that you asked for.
                // Deal with the exception here, and/or rethrow at your discretion.
                throw;
            }
            catch (System.AggregateException ae)
            {
                foreach(var e in ae.InnerExceptions)
                    _logger.Error($"{e.Message} at {e.StackTrace}");
                // More than one of the above exceptions were triggered.
                // Deal with the exception here, and/or rethrow at your discretion.
                throw;
            }
            catch (Exception e)
            {
                _logger.Error($"{e.Message} at {e.StackTrace}");
                
                throw;
            }

            // Decrypts secret using the associated KMS CMK.
            // Depending on whether the secret is a string or binary, one of these fields will be populated.
            if (response.SecretString != null)
            {
                try
                {
                    var secretValue = response.SecretString;

                    _dictionary.Add(secret, secretValue);
                    _logger.Info($"secret recovered");

                    return secretValue;
                }
                catch (Exception ex)
                {
                    _logger.Error($"Cannot recovered the database secretkey {ex.InnerException}");
                    throw;
                }
            }
            else
            {
                memoryStream = response.SecretBinary;
                StreamReader reader = new StreamReader(memoryStream);
                string decodedBinarySecret = Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd()));
            }

            return secret;
        }
    }

    public class SecretManagerModel
    {        
        public string Username { get; set; }
        public string Password { get; set; }
        public string DbInstanceIdentifier { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
        public string Dbname { get; set; }
    }
}
