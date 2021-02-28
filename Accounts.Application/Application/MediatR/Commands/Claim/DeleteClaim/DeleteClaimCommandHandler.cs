using Accounts.Application.MediatR.Base;
using Accounts.Domain.DataAccess.Repositories.Aggregation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Accounts.Application.Application.MediatR.Commands.Claim.DeleteClaim
{
    public class DeleteClaimCommandHandler : AbstractRequestHandler<DeleteClaimCommand>
    {
        private readonly IRoleClaimAggregationRepository _roleClaimAggregationRepository;

        public DeleteClaimCommandHandler(IRoleClaimAggregationRepository roleClaimAggregationRepository)
        {
            _roleClaimAggregationRepository = roleClaimAggregationRepository;
        }

        internal override HandleResponse HandleIt(DeleteClaimCommand request, CancellationToken cancellationToken)
        {
            _roleClaimAggregationRepository.DeleteClaim(request.Id).GetAwaiter().GetResult();

            return new HandleResponse() { Content = true };
        }
    }
}
