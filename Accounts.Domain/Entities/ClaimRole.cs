using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Domain.Entities
{
    public class ClaimRole
    {
        public Claim Claim { get; set; }
        public Role Role { get; set; }
    }
}
