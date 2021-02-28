using Accounts.Application.MediatR.Base;
using Accounts.Domain.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Accounts.Application.Application.MediatR.Commands.Claim.CreateClaim
{
    public class CreateClaimCommandHandler : AbstractRequestHandler<CreateClaimCommand>
    {
        private readonly IClaimRepository _claimRepository;

        public CreateClaimCommandHandler(IClaimRepository claimRepository)
        {
            _claimRepository = claimRepository;
        }

        internal override HandleResponse HandleIt(CreateClaimCommand request, CancellationToken cancellationToken)
        {
            var claim = new Domain.Entities.Claim()
            {
                ClaimType = request.ClaimType,
                ClaimValue = request.ClaimValue,
                Name = request.Name
            };

            var id = _claimRepository.InsertAsync(claim).Result;
            claim.Id = id;

            return new HandleResponse() { Content = claim, ContentIdentification = claim.Id };
        }
    }
}
