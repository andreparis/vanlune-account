using Accounts.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Application.Application.MediatR.Commands.Role.GetAllRoles
{
    public class GetAllRolesCommand : IRequest<Response>
    {
    }
}
