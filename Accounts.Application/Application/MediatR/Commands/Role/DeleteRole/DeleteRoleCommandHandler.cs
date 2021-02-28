using Accounts.Application.MediatR.Base;
using Accounts.Domain.DataAccess.Repositories.Aggregation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Accounts.Application.Application.MediatR.Commands.Role.DeleteRole
{
    public class DeleteRoleCommandHandler : AbstractRequestHandler<DeleteRoleCommand>
    {
        private readonly IRoleClaimAggregationRepository _roleClaimAggregationRepository;

        public DeleteRoleCommandHandler(IRoleClaimAggregationRepository roleClaimAggregationRepository)
        {
            _roleClaimAggregationRepository = roleClaimAggregationRepository;
        }

        internal override HandleResponse HandleIt(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            _roleClaimAggregationRepository.DeleteRole(request.Id).GetAwaiter().GetResult();

            return new HandleResponse() { Content = true };
        }
    }
}
