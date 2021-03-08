using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Domain.Messaging
{
    public interface ISnsClient
    {
        Task Send(string topicArn, string message);
    }
}
