using Accounts.Application.Application.PatchHandlers;
using Accounts.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Application.Application.MediatR.Commands.Role.RolePatch
{
    public class RolePatchCommand : IRequest<Response>
    {
        public int RoleId { get; set; }
        public ICollection<JsonPatch> Patches { get; set; }
    }
}
