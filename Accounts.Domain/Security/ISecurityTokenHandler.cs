using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Domain.Security
{
    public interface ISecurityTokenHandler
    {
        string WriteToken(string username, IEnumerable<System.Security.Claims.Claim> claims, string issuer, string securityKey, DateTime expires);
    }
}
