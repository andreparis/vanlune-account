using Accounts.Domain.DataAccess.Repositories;
using Accounts.Domain.DataAccess.Repositories.Aggregation;
using Accounts.Domain.Entities;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Infrastructure.DataAccess.Database.Aggregation
{
    public class RoleClaimAggregationRepository : IRoleClaimAggregationRepository
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IRoleClaimRepository _roleClaimRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IClaimRepository _claimRepository;
        private readonly IMySqlConnHelper _mySqlConnHelper;

        public RoleClaimAggregationRepository(IRoleRepository roleRepository,
            IRoleClaimRepository roleClaimRepository,
            IUserRoleRepository userRoleRepository,
            IAccountRepository accountRepository,
            IClaimRepository claimRepository,
            IMySqlConnHelper mySqlConnHelper)
        {
            _roleRepository = roleRepository;
            _roleClaimRepository = roleClaimRepository;
            _userRoleRepository = userRoleRepository;
            _accountRepository = accountRepository;
            _claimRepository = claimRepository;
            _mySqlConnHelper = mySqlConnHelper;
        }


        #region Account Roles
        public async Task AddUserRoles(User user)
        {
            var userId = await _accountRepository.InsertAccount(user).ConfigureAwait(false);

            if (userId <= 0)
                return;

            await _userRoleRepository.AddUserRoles(userId, user.Roles.Select(x => x.Role.Id).ToArray()).ConfigureAwait(false);
        }

        public async Task<User> GetUserById(int id)
        {
            return await _accountRepository.GetAccount(id).ConfigureAwait(false);
        }

        public async Task UpdateAccountRoles(User user)
        {
            await _accountRepository.UpdateAccount(user).ConfigureAwait(false);
            await _userRoleRepository.DeleteRolesUserByUserId(user.Id).ConfigureAwait(false);
            await _userRoleRepository.AddUserRoles(user.Id, user.Roles.Select(x => x.Role.Id).ToArray()).ConfigureAwait(false);

        }

        public async Task AttachRolesToUser(User user)
        {
            await _userRoleRepository.DeleteRolesUserByUserId(user.Id).ConfigureAwait(false);
            await _userRoleRepository.AddUserRoles(user.Id, user.Roles.Select(x => x.Role.Id).ToArray()).ConfigureAwait(false);
        }
        
        #endregion

        #region Role Claims
        public async Task AddRoleClaims(Role role)
        {
            var roleId = await _roleRepository.InsertAsync(role).ConfigureAwait(false);

            if (roleId <= 0)
                return;

            await _userRoleRepository.AddUsersRole(role.Users.Select(x => x.User.Id).ToArray(), roleId).ConfigureAwait(false);
            await _roleClaimRepository.AddRoleClaims(roleId, role.Claims.Select(x => x.Claim.Id).ToArray()).ConfigureAwait(false);
        }

        public async Task AttachClaimUsersToRole(Role role)
        {
            await _userRoleRepository.DeleteRolesUserByRoleId(role.Id).ConfigureAwait(false);
            await _roleClaimRepository.DeleteRoleClaimByRoleId(role.Id).ConfigureAwait(false);

            await _userRoleRepository.AddUsersRole(role.Users.Select(x => x.User.Id).ToArray(), role.Id).ConfigureAwait(false);
            await _roleClaimRepository.AddRoleClaims(role.Id, role.Claims.Select(x => x.Claim.Id).ToArray()).ConfigureAwait(false);
        }

        public async Task DeleteRole(int roleId)
        {
            await _userRoleRepository.DeleteRolesUserByRoleId(roleId).ConfigureAwait(false);
            await _roleClaimRepository.DeleteRoleClaimByRoleId(roleId).ConfigureAwait(false);
            await _roleRepository.Delete(roleId).ConfigureAwait(false);
        }

        public async Task DeleteClaim(int claimId)
        {
            await _roleClaimRepository.DeleteRoleClaimByClaimId(claimId).ConfigureAwait(false);
            await _claimRepository.Delete(claimId).ConfigureAwait(false);
        }

        #endregion

        #region Role

        public async Task<Role> GetRoleById(int id)
        {
            return await _roleRepository.GetRole(id).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Role>> GetRolesById(int[] ids)
        {
            return await _roleRepository.GetRolesByIdAsync(ids).ConfigureAwait(false);
        }

        #endregion
    }
}
