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

            services.AddScoped<ISecurityTokenHandler, JwtSecurityTokenHandler>();
            services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();

            return services;
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
