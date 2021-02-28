using System.Collections.Generic;

namespace Accounts.Domain.Entities.CommandsInterface
{
    public interface ICreateUserCommand
    {
        string Name { get; set; }
        string Password { get; set; }
        bool IsActive { get; set; }
        string Email { get; set; }
        string Phone { get; set; }
        string Country { get; set; }
        IEnumerable<Character> Characters { get; set; }
        ICollection<int> IdRoles { get; set; }
    }
}
