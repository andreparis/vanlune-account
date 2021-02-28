using Accounts.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Application.Application.MediatR.Commands.Role.UpdateRole
{
    public class UpdateRoleCommand : IRequest<Response>
    {
        public Domain.Entities.Role Role { get; set; }
    }
}
