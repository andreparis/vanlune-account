using Accounts.Domain.Messaging.Redis;
using Accounts.Infraestructure.Logging;
using Polly;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System;
using System.Collections.Generic;

namespace Accounts.Infraestructure.Messaging.Redis
{
    public class RedisClientHelper<T> : IRedisClientHelper<T>
    {
        private readonly ILogger _logger;
        private readonly IRedisCacheClient _cacheClient;
        private const int MAX_RETRY = 3;

        public RedisClientHelper(IRedisCacheClient cacheClient,
            ILogger logger)
        {
            _cacheClient = cacheClient;
            _logger = logger;
        }

        public T GetFromRedis(string key)
        {
            T result = default;
            try
            {
                var retryPolicy = Policy
                    .Handle<Exception>()
                    .Retry(MAX_RETRY, (exception, retryCount) =>
                    {
                        _logger.Warning($"Exception occurred {exception.Message} when get from Redis - {retryCount} retry");
                    });

                retryPolicy.Execute(() =>
                {
                    result = _cacheClient.Db0.GetAsync<T>(key).Result;
                });
            }
            catch (Exception e)
            {
                _logger.Error($"Erro inesperado {e.Message}");
            }
            return result;
        }

        public IDictionary<string, T> GetAllFromRedis<T>(IEnumerable<string> keys)
        {
            IDictionary<string, T> result = new Dictionary<string, T>();
            try
            {
                var retryPolicy = Policy
                    .Handle<Exception>()
                    .Retry(MAX_RETRY, (exception, retryCount) =>
                    {
                        _logger.Warning($"Exception occurred {exception.Message} when get all from Redis - {retryCount} retry");
                    });

                retryPolicy.Execute(() =>
                {
                    result = _cacheClient.Db0.GetAllAsync<T>(keys).Result;
                });
            }
            catch (Exception e)
            {
                _logger.Error($"Erro inesperado {e.Message}");
            }
            return result;
        }

        public void RemoveInRedis(string key)
        {
            try
            {
                var retryPolicy = Policy
                    .Handle<Exception>()
                    .Retry(MAX_RETRY, (exception, retryCount) =>
                    {
                        _logger.Warning($"Exception occurred {exception.Message} when remove in Redis - {retryCount} retry");
                    });

                retryPolicy.Execute(() =>
                {
                    var result = _cacheClient.Db0.RemoveAsync(key).Result;
                });
            }
            catch (Exception e)
            {
                _logger.Error($"Erro inesperado {e.Message}");
            }
        }

        public void AddRedis(string key, object value, TimeSpan? expiresIn = null)
        {
            try
            {
                var retryPolicy = Policy
                    .Handle<Exception>()
                    .Retry(MAX_RETRY, (exception, retryCount) =>
                    {
                        _logger.Warning($"Exception occurred {exception.Message} when added to Redis - {retryCount} retry");
                    });

                retryPolicy.Execute(() =>
                {
                    bool res;
                    if (expiresIn != null)
                        res = _cacheClient.Db0.AddAsync(key, value, expiresIn: expiresIn.Value).Result;
                    else
                        res = _cacheClient.Db0.AddAsync(key, value).Result;
                });
            }
            catch (Exception e)
            {
                _logger.Error($"Erro inesperado {e.Message}");
            }
        }
    
        public bool Exists(string key)
        {
            try
            {
                var retryPolicy = Policy
                    .Handle<Exception>()
                    .Retry(MAX_RETRY, (exception, retryCount) =>
                    {
                        _logger.Warning($"Exception occurred {exception.Message} when added to Redis - {retryCount} retry");
                    });

                retryPolicy.Execute(() =>
                {
                    return _cacheClient.Db0.ExistsAsync(key).Result;
                });
            }
            catch (Exception e)
            {
                _logger.Error($"Erro inesperado {e.Message}");
            }

            return false;
        }

    }
}
