using Accounts.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Application.Application.MediatR.Commands.Claim.DeleteClaim
{
    public class DeleteClaimCommand : IRequest<Response>
    {
        public int Id { get; set; }
    }
}
