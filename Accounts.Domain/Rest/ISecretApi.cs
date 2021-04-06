using System.Threading.Tasks;

namespace Accounts.Domain.Rest
{
    public interface ISecretApi
    {
        Task<string> GetSecretAsync(string secret);
    }
}
