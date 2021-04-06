using Accounts.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Domain.DataAccess.Repositories
{
    public interface IRoleRepository
    {
        Task<int> InsertAsync(Role role);
        Task<int> UpdateAsync(Role role);
        Task<IEnumerable<Role>> GetAllRoles();
        Task<Role> GetRole(int id);
        Task<Role> GetRoleIdByName(string name);
        Task<IEnumerable<Role>> GetRolesByIdAsync(int[] ids);
        Task Delete(int id);
    }
}
