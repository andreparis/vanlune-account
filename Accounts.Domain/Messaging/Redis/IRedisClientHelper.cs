using System;
using System.Collections.Generic;

namespace Accounts.Domain.Messaging.Redis
{
    public interface IRedisClientHelper<T>
    {
        void AddRedis(string key, object value, TimeSpan? expiresIn = null);
        T GetFromRedis(string key);
        IDictionary<string, T> GetAllFromRedis<T>(IEnumerable<string> keys);
        void RemoveInRedis(string key);
        bool Exists(string key);
    }
}
