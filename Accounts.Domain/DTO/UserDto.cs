using Accounts.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Domain.DTO
{
    public class UserDto : Entity
    {
        public int RoleId { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public IEnumerable<Character> Characters { get; set; }
    }
}
