using Accounts.Domain.DataAccess.Repositories;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Infrastructure.DataAccess.Database
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly IMySqlConnHelper _mySqlConnHelper;

        public UserRoleRepository(IMySqlConnHelper mySqlConnHelper)
        {
            _mySqlConnHelper = mySqlConnHelper;
        }

        public async Task AddUsersRole(int[] usersId, int roleId)
        {
            using var connection = _mySqlConnHelper.MySqlConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            var transaction = connection.BeginTransaction();

            try
            {
                foreach (var userId in usersId)
                {
                    var query = $@"INSERT INTO `Vanlune`.`RoleUser`
                        (`idUser`,
                        `idRoles`)
                        SELECT * FROM 
                        (SELECT @idUser,@idRoles) AS tmp
                        WHERE NOT EXISTS (
                            SELECT idUser,idRoles  
                            FROM Vanlune.RoleUser 
                            WHERE idUser=@idUser 
                            AND idRoles=@idRoles
                        );";
                    await connection.ExecuteAsync(query, new
                    {
                        idUser = userId,
                        idRoles = roleId
                    });
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
            }
        }

        public async Task AddUserRoles(int userId, int[] rolesId)
        {
            using var connection = _mySqlConnHelper.MySqlConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            var transaction = connection.BeginTransaction();
            try
            {
                foreach (var roleId in rolesId)
                {
                    var query = $@"INSERT INTO `Vanlune`.`RoleUser`
                        (`idUser`,
                        `idRoles`)
                        SELECT * FROM 
                        (SELECT @idUser,@idRoles) AS tmp
                        WHERE NOT EXISTS (
                            SELECT idUser,idRoles  
                            FROM Vanlune.RoleUser 
                            WHERE idUser=@idUser 
                            AND idRoles=@idRoles
                        );";
                    await connection.ExecuteAsync(query, new
                    {
                        idUser = userId,
                        idRoles = roleId
                    });
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
            }
        }
    
        public async Task DeleteRolesUser(int idUser, int[] idRoles)
        {
            var query = $@"DELETE FROM `Vanlune`.`RoleUser`
                        WHERE idUser = @idUser
                        AND idRoles IN @idRoles";

            using var connection = _mySqlConnHelper.MySqlConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            await connection.ExecuteAsync(query, new
            {
                idUser,
                idRoles
            });
        }

        public async Task DeleteRolesUserByRoleId(int idRoles)
        {
            var query = $@"DELETE FROM `Vanlune`.`RoleUser`
                        WHERE idRoles = @idRoles";

            using var connection = _mySqlConnHelper.MySqlConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            await connection.ExecuteAsync(query, new
            {
                idRoles
            });
        }

        public async Task DeleteRolesUserByUserId(int idUser)
        {
            var query = $@"DELETE FROM `Vanlune`.`RoleUser`
                        WHERE idUser = @idUser";

            using var connection = _mySqlConnHelper.MySqlConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            await connection.ExecuteAsync(query, new
            {
                idUser
            });
        }
    }
}
