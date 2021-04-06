using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Domain.Rest
{
    public interface ISnsApi
    {
        Task<string> SendEmailAsyn(string topicArn, string message);
    }
}
