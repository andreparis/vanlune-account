using Accounts.Domain.Entities;
using Accounts.Domain.Messaging;
using Accounts.Infraestructure.Logging;
using Accounts.Infraestructure.Messaging.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Infrastructure.Messaging.Redis
{
    public class AuthRedis : RedisClientHelper<Credentials>, IAuthRedis
    {
        public AuthRedis(IRedisCacheClient cacheClient,
            ILogger logger) : base(cacheClient, logger) { }

        public Credentials GetCredentials(string token)
        {
            return GetFromRedis(string.Concat("user:", token));
        }

        public void AddAuth(string token, Credentials credentials)
        {
            AddRedis(token, 
                credentials, 
                TimeSpan.FromMinutes(30));
        }
    }
}
