using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Domain.Entities
{
    public class Claim : Entity
    {
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public virtual ICollection<ClaimRole> Roles { get; } = new List<ClaimRole>();
    }
}
