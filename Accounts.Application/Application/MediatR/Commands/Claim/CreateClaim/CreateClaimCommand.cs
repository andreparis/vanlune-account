using Accounts.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Application.Application.MediatR.Commands.Claim.CreateClaim
{
    public class CreateClaimCommand : IRequest<Response>
    {
        public string Name { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
}
