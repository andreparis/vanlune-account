using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accounts.Domain.Entities
{
    public class Role : Entity
    {
        public virtual ICollection<ClaimRole> Claims { get; } = new List<ClaimRole>();

        public void Create(string name, IEnumerable<int> claims)
        {
            Name = name;

            if (claims != null)
                foreach (var claim in claims)
                    Claims.Add(new ClaimRole { Claim = new Claim() { Id = claim } });
        }

        public void Update(string name)
        {
            Name = name;
        }

        public void AddClaimToRole(Claim claim)
        {
            if (!Claims.Any(x => x.Claim.Id.Equals(claim.Id)))
                Claims.Add(new ClaimRole() { Claim = claim });
        }

        public void RemoveClaimFromRole(Claim claim)
        {
            var existingClaim = Claims.FirstOrDefault(r => r.Claim.Id == claim.Id);

            if (existingClaim == null)
                throw new Exception($"Claim {claim.Id} not found");

            Claims.Remove(existingClaim);
        }
    }
}
