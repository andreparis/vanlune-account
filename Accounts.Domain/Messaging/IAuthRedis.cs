using Accounts.Domain.Entities;
using Accounts.Domain.Messaging.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Domain.Messaging
{
    public interface IAuthRedis : IRedisClientHelper<Credentials>
    {
        Credentials GetCredentials(string token);
        void AddAuth(string token, Credentials credentials);
    }
}
