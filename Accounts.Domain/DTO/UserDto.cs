using Accounts.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Domain.DTO
{
    public class UserDto : Entity
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<Character> Characters { get; set; }
    }
}
