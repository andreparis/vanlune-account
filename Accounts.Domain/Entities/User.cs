using Accounts.Domain.Entities.CommandsInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accounts.Domain.Entities
{
    public class User : Entity
    {
        public string PasswordHash { get; private set; }

        public ICollection<UserRole> Roles { get; } = new List<UserRole>();

        public string Email { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public IEnumerable<Character> Characters { get; set; }

        public void Create(ICreateUserCommand command, IEnumerable<Role> roles)
        {
            this.SetDomainValues(command);
            this.AddRoles(roles);
        }

        public void AddRoles(IEnumerable<Role> roles)
        {
            if (roles == null)
                return;

            foreach (var role in roles)
            {
                if (!this.Roles.Any(x => x.Role.Id.Equals(role.Id)))
                    this.Roles.Add(new UserRole { Role = role });
            }                
        }

        private void SetDomainValues(ICreateUserCommand command)
        {
            if (!string.IsNullOrEmpty(command.Name))
                this.Name = command.Name;

            //if (!string.IsNullOrEmpty(command.Email))
            //    this.Email = command.Email;

            if (!string.IsNullOrEmpty(command.Password))
                this.PasswordHash = command.Password;

            if (!string.IsNullOrEmpty(command.Country))
                this.Country = command.Country;

            if (!string.IsNullOrEmpty(command.Phone))
                this.Phone = command.Phone;

            if (command.Characters != null && command.Characters.Count() > 0)
                this.Characters = command.Characters;
        }

        public void Update(IUpdateUserCommand command)
        {
            this.SetDomainValues(command);
        }

        #region Manipulate Roles       
        public ICollection<Role> GetJustTheRoles()
        {
            return this.Roles?.Select(r => r.Role).ToList();
        }

        public void DesassociateRolesRange(IEnumerable<Role> roles)
        {
            if (roles == null)
                throw new ArgumentNullException(nameof(roles));

            foreach (var role in roles)
            {
                this.RemoveUserFromRole(role.Id);
            }
        }
        #endregion

        public void AddRoleToUser(Role role)
        {
            if (!Roles.Any(x => x.Role.Id.Equals(role.Id)))
                this.Roles.Add(new UserRole { Role = role });
        }

        public void RemoveUserFromRole(int roleId)
        {
            var existingRole = this.Roles?.FirstOrDefault(r => r?.Role?.Id == roleId);

            if (existingRole == null)
                return;

            this.Roles.Remove(existingRole);
        }

        public void ChangePassword(string passwordHash)
        {
            this.PasswordHash = passwordHash;
        }
    }
}
