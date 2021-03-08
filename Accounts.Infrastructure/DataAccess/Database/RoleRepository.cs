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
    public class RoleRepository : IRoleRepository
    {
        private readonly IMySqlConnHelper _mySqlConnHelper;

        public RoleRepository(IMySqlConnHelper mySqlConnHelper)
        {
            _mySqlConnHelper = mySqlConnHelper;
        }

        public async Task<int> InsertAsync(Role role)
        {
            var query = $@"INSERT INTO `Vanlune`.`Roles`
                        (`name`)
                        VALUES
                        (@{nameof(Role.Name)});
                        SELECT LAST_INSERT_ID();";

            using var connection = _mySqlConnHelper.MySqlConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            var result = await connection.QueryAsync<int>(query, new
            {
                role.Name
            });

            return result.Single();
        }

        public async Task<int> UpdateAsync(Role role)
        {
            var query = $@"UPDATE `Vanlune`.`Roles`
                        SET
                        `name` = {nameof(Role.Name)}
                        WHERE `id` = {nameof(Role.Id)};";

            using var connection = _mySqlConnHelper.MySqlConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            var result = await connection.QueryAsync<int>(query, new
            {
                role.Name,
                role.Id
            });

            return result.Single();
        }

        public async Task<Role> GetRole(int id)
        {
            var query = $@"SELECT 
                            R.`id`     AS {nameof(Role.Id)},
                            R.`name`   AS {nameof(Role.Name)},
                            A.id       AS {nameof(User.Id)},
                            A.name     AS {nameof(User.Name)},   
                            C.id       AS {nameof(Claim.Id)},
                            C.name     AS {nameof(Claim.Name)}                            
                            FROM Vanlune.Roles AS R
                            LEFT JOIN `Vanlune`.RoleUser AS AR ON AR.idRoles = R.id
                            LEFT JOIN `Vanlune`.`Accounts` AS A ON A.id = AR.idUser
                            LEFT JOIN `Vanlune`.ClaimRole AS CR ON CR.idRole = R.id
                            LEFT JOIN `Vanlune`.Claim AS C ON C.id = CR.idClaim
                            WHERE R.`id` = @id";

            using var connection = _mySqlConnHelper.MySqlConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            Role currRole = default;

            var result = await connection.QueryAsync<Role, User, Claim, Role>(query, 
            (role, user, claim) => 
            {
                if (currRole == null)
                    currRole = role;
                if (claim != null && claim.Id > 0)
                    currRole.AddClaimToRole(claim);

                return role;
            },
            new
            {
                id
            }, splitOn: $"{nameof(Role.Id)},{nameof(User.Id)},{nameof(Claim.Id)}");

            return currRole;
        }

        public async Task<Role> GetRoleIdByName(string name)
        {
            var query = $@"SELECT 
                            R.`id`     AS {nameof(Role.Id)},
                            R.`name`   AS {nameof(Role.Name)}                           
                            FROM Vanlune.Roles AS R
                            WHERE R.`name` = @name";

            using var connection = _mySqlConnHelper.MySqlConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            var result = await connection.QueryAsync<Role>(query,
            new
            {
                name
            });

            return result.Single();
        }

        public async Task<IEnumerable<Role>> GetRolesByIdAsync(int[] ids)
        {
            var query = $@"SELECT 
                            R.`id`     AS {nameof(Role.Id)},
                            R.`name`   AS {nameof(Role.Name)}
                            FROM `Vanlune`.`Roles` AS R
                            WHERE R.`id` IN @ids";

            using var connection = _mySqlConnHelper.MySqlConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            var result = await connection.QueryAsync<Role>(query, new
            {
                ids
            });

            return result;
        }

        public async Task Delete(int id)
        {
            var query = $@"DELETE FROM `Vanlune`.`Roles`
                        WHERE R.`id` = @id";

            using var connection = _mySqlConnHelper.MySqlConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            await connection.ExecuteAsync(query, new
            {
                id
            }).ConfigureAwait(false);
        }
    }
}
