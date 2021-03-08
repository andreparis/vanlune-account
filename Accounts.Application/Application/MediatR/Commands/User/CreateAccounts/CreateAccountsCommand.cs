using MediatR;
using Accounts.Domain.Entities;
using Accounts.Domain.Entities.CommandsInterface;
using System.Collections.Generic;

namespace Accounts.Application.Application.MediatR.Commands.CreateProducts
{
    public class CreateAccountsCommand : IRequest<Response>, ICreateUserCommand
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public IEnumerable<Character> Characters { get; set; }
        public ICollection<int> IdRoles { get; set; }
    }
}
