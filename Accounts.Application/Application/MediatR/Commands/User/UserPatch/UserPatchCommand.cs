using Accounts.Application.Application.PatchHandlers;
using Accounts.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Application.Application.MediatR.Commands.User.UserPatch
{
    public class UserPatchCommand : IRequest<Response>
    {
        public int UserId { get; set; }
        public ICollection<JsonPatch> Patches { get; set; }
    }
}
