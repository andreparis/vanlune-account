using AutoMapper;
using Accounts.Domain.DataAccess.S3;
using Accounts.Infraestructure.DataAccess.S3;
using Accounts.Infraestructure.Logging;
using Accounts.Infraestructure.Security;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Reflection;
using System;
using Accounts.Domain.DataAccess.Repositories;
using Accounts.Infrastructure.DataAccess.Database.Base;
using Accounts.Infrastructure.DataAccess.Database;
using Accounts.Domain.Security;
using Accounts.Infrastructure.Security;
using Accounts.Domain.DataAccess.Repositories.Aggregation;
using Accounts.Infrastructure.DataAccess.Database.Aggregation;
using StackExchange.Redis.Extensions.Core.Configuration;
using System.Collections.Generic;
using StackExchange.Redis.Extensions.Newtonsoft;
using StackExchange.Redis.Extensions.Core.Implementations;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System.Diagnostics;
using Accounts.Infrastructure.Messaging.Redis;
using Accounts.Domain.Messaging;
using Accounts.Infrastructure.Messaging.SNS;

namespace Accounts.Application.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services)
        {
            IConfigurationRoot configuration = GetConfiguration();
            services.AddSingleton<IConfiguration>(configuration);

#if DEBUG
            services.AddDefaultAWSOptions(configuration.GetAWSOptions());
#endif
            services.AddMediatR(AppDomain.CurrentDomain.Load("Accounts.Application"));
            services.AddAutoMapper(typeof(Function).Assembly);
            services.AddSingleton<ILogger, Logger>();
            services.AddSingleton<IAwsSecretManagerService, AwsSecretManagerService>();

            services.AddSingleton<IMySqlConnHelper, MySqlConnHelper>();
            services.AddSingleton<IAccountRepository, AccountRepository>();
            services.AddSingleton<IRoleRepository, RoleRepository>();
            services.AddSingleton<IClaimRepository, ClaimRepository>();
            services.AddSingleton<IRoleClaimRepository, RoleClaimRepository>();
            services.AddSingleton<IUserRoleRepository, UserRoleRepository>();
            services.AddSingleton<IRoleClaimAggregationRepository, RoleClaimAggregationRepository>();
            
            services.AddSingleton<Domain.Messaging.ISnsClient, SnsClient>();

            services.AddScoped<ISecurityTokenHandler, JwtSecurityTokenHandler>();
            services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();

            //if (Debugger.IsAttached)
            //    services.AddRedis(configuration["Redis:InlineHosts"], configuration["Redis:Password"]);
            //else
            //    services.AddRedis(configuration["Redis_InlineHosts"], configuration["Redis_Password"]);

            return services;
        }

        public static void AddRedis(this IServiceCollection services,
            string hostsInLine,
            string password,
            bool abortOnConnectFail = true,
            int syncTimeout = 30)
        {
            var newRedisConfiguration = new RedisConfiguration()
            {
                AbortOnConnectFail = abortOnConnectFail,
                Password = password,
                Ssl = true
            };

            if (!string.IsNullOrEmpty(hostsInLine))
            {
                var hosts = new List<RedisHost>();

                var splitted = hostsInLine.Split(' ');
                for (int i = 0; i < splitted.Length - 1; i += 2)
                {
                    var host = new RedisHost()
                    {
                        Host = splitted[i],
                        Port = Convert.ToInt32(splitted[i + 1])
                    };

                    hosts.Add(host);
                }

                newRedisConfiguration.Hosts = hosts.ToArray();
            }

            newRedisConfiguration.ConfigurationOptions.SyncTimeout = Convert.ToInt32(syncTimeout);

            services.AddStackExchangeRedisExtensions<NewtonsoftSerializer>(newRedisConfiguration);
            services.AddSingleton<IRedisCacheClient, RedisCacheClient>();
            services.AddSingleton<IRedisCacheConnectionPoolManager, RedisCacheConnectionPoolManager>();
        }

        private static IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile($"appsettings.json")
                            .AddEnvironmentVariables();

            var configuration = builder.Build();
            return configuration;
        }
    }
}
