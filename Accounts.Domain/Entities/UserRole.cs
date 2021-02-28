using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Domain.Entities
{
    public class UserRole
    {
        public Role Role { get; set; }
        public User User { get; set; }
    }
}
