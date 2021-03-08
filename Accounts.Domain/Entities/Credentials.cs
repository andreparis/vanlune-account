using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Domain.Entities
{
    public class Credentials
    {
        public IEnumerable<Role> Roles { get; set; }
    }
}
