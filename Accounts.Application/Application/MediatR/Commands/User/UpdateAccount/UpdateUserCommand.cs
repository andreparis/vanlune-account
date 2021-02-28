using Accounts.Domain.Entities;
using Accounts.Domain.Entities.CommandsInterface;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Application.Application.MediatR.Commands.User.UpdateAccount
{
    public class UpdateUserCommand : IRequest<Response>, IUpdateUserCommand
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public IEnumerable<Character> Characters { get; set; }
        public ICollection<int> IdRoles { get; set; }

    }
}
