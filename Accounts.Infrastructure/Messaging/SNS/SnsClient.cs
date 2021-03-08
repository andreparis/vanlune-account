using Accounts.Domain.Messaging;
using Accounts.Infraestructure.Logging;
using Amazon;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Infrastructure.Messaging.SNS
{
    public class SnsClient : ISnsClient
    {
        private readonly IAmazonSimpleNotificationService _client;
        private readonly ILogger _logger;

        public SnsClient(IConfiguration configuration,
            ILogger logger)
        {
            if (Debugger.IsAttached)
            {
                _client = new AmazonSimpleNotificationServiceClient(new BasicAWSCredentials(configuration["AWS:AccessKey"], configuration["AWS:SecretKey"]), RegionEndpoint.USEast1);
            }
            else
            {
                _client = new AmazonSimpleNotificationServiceClient(RegionEndpoint.USEast1);
            }
            _logger = logger;
        }

        public async Task Send(string topicArn, string message)
        {
            _logger.Info($"Sending message to {topicArn}");

            try
            {
                var request = new PublishRequest
                {
                    Message = message,
                    TargetArn = topicArn
                };

                var response = await _client.PublishAsync(request).ConfigureAwait(false);

                _logger.Info($"Sent to {topicArn} with id {response.MessageId}");
            }
            catch (Exception e)
            {
                _logger.Info($"Error {e.Message} at {e.StackTrace}");
            }
        }
    }
}
