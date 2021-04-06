using Accounts.Domain.DataAccess.Repositories;
using Accounts.Domain.DataAccess.Repositories.Base;
using Accounts.Infraestructure.Security;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Accounts.Infrastructure.DataAccess.Database.Base
{
    public class MySqlConnHelper : IMySqlConnHelper
    {
        private readonly string _connectionString;

        public MySqlConnHelper(IConfiguration configuration,
            IAwsSecretManagerService awsSecretManagerService)
        {
            var secretObj = awsSecretManagerService.GetSecret(configuration["CONN_STRING"]);
            var secret = JsonConvert.DeserializeObject<SecretDb>(secretObj);
            _connectionString = $@"server={secret.Host};
                                userid={secret.Username};
                                password={secret.Password};
                                database=Vanlune;
                                Pooling=True;
                                Min Pool Size=0;
                                Max Pool Size=5;
                                Connection Lifetime=60; 
                                default command timeout=300;";
        }

        public DbConnection MySqlConnection()
        {
            return new MySql.Data.MySqlClient.MySqlConnection(_connectionString);
        }
    }
}
