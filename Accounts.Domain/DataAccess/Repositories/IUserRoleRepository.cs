using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Domain.DataAccess.Repositories
{
    public interface IUserRoleRepository
    {
        Task AddUsersRole(int[] usersId, int roleId);
        Task AddUserRoles(int userId, int[] rolesId);
        Task DeleteRolesUser(int idUser, int[] idRoles);
        Task DeleteRolesUserByRoleId(int idRoles);
        Task DeleteRolesUserByUserId(int idUser);
    }
}
