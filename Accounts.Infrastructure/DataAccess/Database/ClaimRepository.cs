using Accounts.Domain.DataAccess.Repositories;
using Accounts.Domain.Entities;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Infrastructure.DataAccess.Database
{
    public class ClaimRepository : IClaimRepository
    {
        private readonly IMySqlConnHelper _mySqlConnHelper;

        public ClaimRepository(IMySqlConnHelper mySqlConnHelper)
        {
            _mySqlConnHelper = mySqlConnHelper;
        }

        public async Task<int> InsertAsync(Claim claim)
        {
            var query = $@"INSERT INTO `Vanlune`.`Claim`
                        (`name`,
                        `type`,
                        `value`)
                        VALUES
                        (@{nameof(Claim.Name)},
                        @{nameof(Claim.ClaimType)},
                        @{nameof(Claim.ClaimValue)});
                        SELECT LAST_INSERT_ID();";

            using var connection = _mySqlConnHelper.MySqlConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            var result = await connection.QueryAsync<int>(query, new
            {
                claim.Name,
                claim.ClaimType,
                claim.ClaimValue
            });

            return result.Single();
        }

        public async Task<IEnumerable<Claim>> GetClaimsByIdAsync(int[] ids)
        {
            var query = $@"SELECT 
                                `Claim`.`id`    AS {nameof(Claim.Id)},
                                `Claim`.`name`  AS {nameof(Claim.Name)},
                                `Claim`.`type`  AS {nameof(Claim.ClaimType)},
                                `Claim`.`value` AS {nameof(Claim.ClaimValue)}
                            FROM `Vanlune`.`Claim`
                            WHERE `Claim`.`id` in @ids";

            using var connection = _mySqlConnHelper.MySqlConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            var result = await connection.QueryAsync<Claim>(query, new
            {
                ids
            });

            return result;
        }

        public async Task Delete(int id)
        {
            var query = $@"DELETE FROM `Vanlune`.`Claim`
                            WHERE `Claim`.`id` = @id";

            using var connection = _mySqlConnHelper.MySqlConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            await connection.ExecuteAsync(query, new
            {
                id
            });
        }
    }
}
