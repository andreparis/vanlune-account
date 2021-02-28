using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accounts.Domain.Entities
{
    public class Role : Entity
    {
        public virtual ICollection<ClaimRole> Claims { get; } = new List<ClaimRole>();
        public virtual ICollection<UserRole> Users { get; } = new List<UserRole>();

        public void Create(string name, IEnumerable<int> users, IEnumerable<int> claims)
        {
            Name = name;

            if (users != null)
                foreach (var user in users)
                    Users.Add(new UserRole { User = new User() { Id = user } });

            if (claims != null)
                foreach (var claim in claims)
                    Claims.Add(new ClaimRole { Claim = new Claim() { Id = claim } });
        }

        public void Update(string name)
        {
            Name = name;
        }

        public void AddUserToRole(User user)
        {
            if (!Users.Any(x => x.User.Id.Equals(user.Id)))
                Users.Add(new UserRole { User = user });
        }

        public void RemoveUserFromRole(User user)
        {
            var existingUser = Users.FirstOrDefault(r => r.User.Id == user.Id);

            if (existingUser == null)
                throw new Exception($"User {user.Id} not found");

            Users.Remove(existingUser);
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
