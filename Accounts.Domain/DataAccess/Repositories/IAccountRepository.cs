using Accounts.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Domain.DataAccess.Repositories
{
    public interface IAccountRepository
    {
        Task<User> GetAccount(int id);
        Task<User> GetAccountByEmail(string email);
        Task<int> InsertAccount(User account);
        Task UpdateAccount(User account);
        Task UpdatePassword(int userId, string password);
    }
}
