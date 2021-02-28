using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Accounts.Infraestructure.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading;

namespace Accounts.Infraestructure.Messaging.SQS
{
    public class AmazonSqsClientHelper : IAmazonSqsClientHelper
    {
        private readonly IAmazonSQS _amazonSQS;
        private readonly IConfiguration _configuration;
        private readonly string _queueUrl;
        private readonly ILogger _logger;

        public AmazonSqsClientHelper(ILogger logger, IConfiguration configuration)
        {
            _configuration = configuration;
            if (Debugger.IsAttached)
                _amazonSQS = new AmazonSQSClient(new BasicAWSCredentials(_configuration["AWS:AccessKey"], _configuration["AWS:SecretKey"]), RegionEndpoint.SAEast1);
            else
                _amazonSQS = new AmazonSQSClient(RegionEndpoint.GetBySystemName(configuration["AWS_REGION"]));
            _queueUrl = "https://sqs." + configuration["AWS_REGION"] + ".amazonaws.com/" + configuration["AWS_ACCOUNT"];
            _logger = logger;
        }

        public SendMessageResponse SendMessageAsync(string queueName, string messageBody)
        {
            var request = new SendMessageRequest(
                queueUrl: string.Concat(_queueUrl, queueName),
                messageBody: messageBody);

            SendMessageResponse result = null;

            try
            {
                result = _amazonSQS.SendMessageAsync(request, new CancellationToken()).Result;

                if (result != null)
                    _logger.Info($"Result sqs posted message: {JsonConvert.SerializeObject(result)}");

                _logger.Info($"Message {messageBody} sent to SQS {string.Concat(_queueUrl, queueName)}. Result: {result?.HttpStatusCode}");
            }
            catch (System.Exception ex)
            {
                _logger.Error($"Error to post message at sqs: {ex.InnerException.Message}");
                _logger.Error(ex.StackTrace);
                throw;
            }

            return result;
        }
    }
}
