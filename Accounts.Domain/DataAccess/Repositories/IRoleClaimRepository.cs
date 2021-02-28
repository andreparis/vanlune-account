using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Domain.DataAccess.Repositories
{
    public interface IRoleClaimRepository
    {
        Task AddRoleClaims(int roleId, int[] claims);
        Task DeleteRoleClaimByRoleId(int idRole);
        Task DeleteRoleClaimByClaimId(int idClaim);
    }
}
