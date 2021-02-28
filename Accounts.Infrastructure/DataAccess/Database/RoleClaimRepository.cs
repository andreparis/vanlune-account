using Accounts.Domain.DataAccess.Repositories;
using Accounts.Infraestructure.Logging;
using Dapper;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Accounts.Infrastructure.DataAccess.Database
{
    public class RoleClaimRepository : IRoleClaimRepository
    {
        private readonly IMySqlConnHelper _mySqlConnHelper;
        private readonly ILogger _logger;

        public RoleClaimRepository(IMySqlConnHelper mySqlConnHelper,
            ILogger logger)
        {
            _mySqlConnHelper = mySqlConnHelper;
            _logger = logger;
        }

        public async Task AddRoleClaims(int roleId, int[] claims)
        {
            using var connection = _mySqlConnHelper.MySqlConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            var transaction = connection.BeginTransaction();

            try
            {

                foreach (var claimId in claims)
                {
                    var query = $@"INSERT INTO `Vanlune`.`ClaimRole`
                                (`idClaim`,
                                `idRole`)
                                VALUES
                                (@claimId,
                                @roleId);";
                    await connection.ExecuteAsync(query, new
                    {
                        claimId,
                        roleId
                    });
                }

                transaction.Commit();
            }
            catch(Exception e)
            {
                _logger.Error($"Error {e.Message} at {e.StackTrace}");

                transaction.Rollback();
            }
        }

        public async Task DeleteRoleClaimByRoleId(int idRole)
        {
            var query = $@"DELETE FROM `Vanlune`.`ClaimRole`
                        WHERE idRole = @idRole";

            using var connection = _mySqlConnHelper.MySqlConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            await connection.ExecuteAsync(query, new
            {
                idRole
            });
        }

        public async Task DeleteRoleClaimByClaimId(int idClaim)
        {
            var query = $@"DELETE FROM `Vanlune`.`ClaimRole`
                        WHERE idClaim = @idClaim";

            using var connection = _mySqlConnHelper.MySqlConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            await connection.ExecuteAsync(query, new
            {
                idClaim
            });
        }
    }
}
