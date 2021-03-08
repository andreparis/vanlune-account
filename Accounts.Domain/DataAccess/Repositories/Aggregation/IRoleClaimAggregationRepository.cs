using Accounts.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Domain.DataAccess.Repositories.Aggregation
{
    public interface IRoleClaimAggregationRepository
    {
        Task AddRoleClaims(Role role);
        Task AddUserRoles(User user);
        Task AttachRolesToUser(User user);
        Task AttachClaimsToRole(Role role);
        Task<User> GetUserById(int id);
        Task UpdateAccountRoles(User user);
        Task<Role> GetRoleById(int id);
        Task<IEnumerable<Role>> GetRolesById(int[] ids);
        Task DeleteClaim(int claimId);
        Task DeleteRole(int roleId);
    }
}
