using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Infraestructure.Security
{
    public interface IAwsSecretManagerService
    {
        string GetSecret(string secret);
    }
}
