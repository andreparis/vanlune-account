using Accounts.Domain.DataAccess.Repositories;
using Accounts.Domain.Entities;
using Accounts.Infrastructure.DataAccess.Extension;
using Dapper;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Accounts.Infrastructure.DataAccess.Database
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IMySqlConnHelper _mySqlConnHelper;

        public AccountRepository(IMySqlConnHelper mySqlConnHelper)
        {
            _mySqlConnHelper = mySqlConnHelper;
            SqlMapper.AddTypeHandler(new DapperCharsTypeHandler());
        }

        public async Task<User> GetAccount(int id)
        {
            var query = $@"SELECT 
                            A.`id`         AS {nameof(User.Id)},
                            A.`name`       AS {nameof(User.Name)},
                            A.`email`      AS {nameof(User.Email)},
                            A.`phone`      AS {nameof(User.Phone)},
                            A.`country`    AS {nameof(User.Country)},
                            A.`characters` AS {nameof(User.Characters)},
                            A.`password`   AS {nameof(User.PasswordHash)},
                            A.`isActive`   AS {nameof(User.IsActive)},
                            R.id           AS {nameof(Role.Id)},
                            R.name         AS {nameof(Role.Name)},
                            C.id           AS {nameof(Claim.Id)},
                            C.name         AS {nameof(Claim.Name)},
                            C.type         AS {nameof(Claim.ClaimType)},
                            C.value        AS {nameof(Claim.ClaimValue)}
                        FROM Vanlune.`Accounts` AS A
                        LEFT JOIN `Vanlune`.RoleUser AS AR ON AR.idUser = A.id
                        LEFT JOIN `Vanlune`.Roles AS R ON R.id = AR.idRoles
                        LEFT JOIN `Vanlune`.ClaimRole AS CR ON CR.idRole = R.id
                        LEFT JOIN `Vanlune`.Claim AS C ON C.id = CR.idClaim
                        WHERE A.`id`= @id";

            using var connection = _mySqlConnHelper.MySqlConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            User currUser = default;
            var result = await connection.QueryAsync<User, Role, Claim, User>(query,
            (user, role, claim) =>
            {
                if (currUser == null)
                    currUser = user;

                if (currUser.Roles.Any(x => x.Role.Id.Equals(role.Id)))
                {
                    role = currUser.Roles.Where(x => x.Role.Id.Equals(role.Id)).FirstOrDefault().Role;
                }
                if (role != null)
                {
                    if (claim != null)
                        role.AddClaimToRole(claim);

                    currUser.AddRoles(new List<Role>() { role });
                }
               

                return user;
            },
             new
             {
                 id
             }, splitOn: $"{nameof(User.Id)},{nameof(Role.Id)},{nameof(Claim.Id)}");

            return currUser;
        }

        public async Task<User> GetAccountByEmail(string email)
        {
            var query = $@"SELECT 
                            A.`id`         AS {nameof(User.Id)},
                            A.`name`       AS {nameof(User.Name)},
                            A.`email`      AS {nameof(User.Email)},
                            A.`phone`      AS {nameof(User.Phone)},
                            A.`country`    AS {nameof(User.Country)},
                            A.`characters` AS {nameof(User.Characters)},
                            A.`password`   AS {nameof(User.PasswordHash)},
                            A.`isActive`   AS {nameof(User.IsActive)},
                            R.id           AS {nameof(Role.Id)},
                            R.name         AS {nameof(Role.Name)},
                            C.id           AS {nameof(Claim.Id)},
                            C.name         AS {nameof(Claim.Name)},
                            C.type         AS {nameof(Claim.ClaimType)},
                            C.value        AS {nameof(Claim.ClaimValue)}
                        FROM Vanlune.`Accounts` AS A
                        LEFT JOIN `Vanlune`.RoleUser AS AR ON AR.idUser = A.id
                        LEFT JOIN `Vanlune`.Roles AS R ON R.id = AR.idRoles
                        LEFT JOIN `Vanlune`.ClaimRole AS CR ON CR.idRole = R.id
                        LEFT JOIN `Vanlune`.Claim AS C ON C.id = CR.idClaim
                        WHERE A.`email`= @email";

            using var connection = _mySqlConnHelper.MySqlConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            User currUser = default;
            var result = await connection.QueryAsync<User, Role, Claim, User>(query, 
            (user, role, claim) => 
            {
                if (currUser == null)
                    currUser = user;                

                if (currUser.Roles.Any(x => x.Role.Id.Equals(role.Id)))
                {
                    role = currUser.Roles.Where(x => x.Role.Id.Equals(role.Id)).FirstOrDefault().Role;
                }
                role.AddClaimToRole(claim);

                currUser.AddRoles(new List<Role>() { role });

                return user;
            },
             new
            {
                email
            }, splitOn: $"{nameof(User.Id)},{nameof(Role.Id)},{nameof(Claim.Id)}");

            return currUser;
        }

        public async Task<int> InsertAccount(User account)
        {
            var query = $@"INSERT INTO `Vanlune`.`Accounts`
                        (`name`,
                        `password`,
                        `email`,
                        `phone`,
                        `country`,
                        `characters`,
                        `isActive`)
                        VALUES
                        (@{nameof(User.Name)},
                        @{nameof(User.PasswordHash)},
                        @{nameof(User.Email)},
                        @{nameof(User.Phone)},
                        @{nameof(User.Country)},
                        @{nameof(User.Characters)},
                        @{nameof(User.IsActive)});
                        SELECT LAST_INSERT_ID();";

            using var connection = _mySqlConnHelper.MySqlConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            var result = await connection.QueryAsync<int>(query, new
            {
                account.Name,
                account.PasswordHash,
                account.Email,
                account.Phone,
                account.Country,
                Characters = JsonConvert.SerializeObject(account.Characters),
                account.IsActive
            });

            return result.Single();
        }
    
        public async Task UpdateAccount(User account)
        {
            var characters = account.Characters?.Count() > 0 ?
                $"`characters` = @{nameof(User.Characters)}," : "";
            var query = $@"UPDATE `Vanlune`.`Accounts`
                            SET
                            `name` = @{nameof(User.Name)},
                            `email` = @{nameof(User.Email)},
                            `phone` = @{nameof(User.Phone)},
                            `country` = @{nameof(User.Country)},
                            {characters}
                            `password` = @{nameof(User.PasswordHash)},
                            `isActive` = @{nameof(User.IsActive)}
                            WHERE `id` = @{nameof(User.Id)};";

            using var connection = _mySqlConnHelper.MySqlConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            var charsList = account.Characters?.Count() > 0 ?
                JsonConvert.SerializeObject(account.Characters) :
                "";

            await connection.ExecuteAsync(query, new
            {
                account.Name,
                account.PasswordHash,
                account.Email,
                account.Phone,
                account.Country,
                account.Id,
                account.IsActive,
                Characters = charsList
            });
        }

        public async Task UpdatePassword(int userId, string password)
        {
            var query = $@"UPDATE `Vanlune`.`Accounts`
                            SET
                            `password` = @password
                            WHERE `id` = @userId;";

            using var connection = _mySqlConnHelper.MySqlConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            await connection.ExecuteAsync(query, new
            {
                userId,
                password
            });
        }
    }
}
