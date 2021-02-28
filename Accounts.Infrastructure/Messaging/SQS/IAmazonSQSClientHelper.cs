using Amazon.SQS.Model;

namespace Accounts.Infraestructure.Messaging.SQS
{
    public interface IAmazonSqsClientHelper
    {
        SendMessageResponse SendMessageAsync(string queueName, string messageBody);
    }
}
