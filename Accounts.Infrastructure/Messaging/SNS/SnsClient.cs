using Accounts.Domain.Messaging;
using Accounts.Domain.Rest;
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
        private readonly ISnsApi _client;
        private readonly ILogger _logger;

        public SnsClient(ISnsApi client,
            ILogger logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task Send(string topicArn, string message)
        {
            _logger.Info($"Sending message to {topicArn}");

            try
            {

                var response = await _client.SendEmailAsyn(topicArn, message);
            }
            catch (Exception e)
            {
                _logger.Info($"Error {e.Message} at {e.StackTrace}");
            }
        }
    }
}
