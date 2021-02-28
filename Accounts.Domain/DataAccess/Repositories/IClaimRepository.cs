using Accounts.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Domain.DataAccess.Repositories
{
    public interface IClaimRepository
    {
        Task<int> InsertAsync(Claim claim);
        Task<IEnumerable<Claim>> GetClaimsByIdAsync(int[] ids);
        Task Delete(int id);
    }
}
