using Accounts.Application.MediatR.Base;
using Accounts.Domain.DataAccess.Repositories.Aggregation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Accounts.Application.Application.MediatR.Commands.Role.CreateRole
{
    public class CreateRoleCommandHandler : AbstractRequestHandler<CreateRoleCommand>
    {
        private readonly IRoleClaimAggregationRepository _roleClaimAggregationRepository;

        public CreateRoleCommandHandler(IRoleClaimAggregationRepository roleClaimAggregationRepository)
        {
            _roleClaimAggregationRepository = roleClaimAggregationRepository;
        }

        internal override HandleResponse HandleIt(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = new Domain.Entities.Role();

            role.Create(
                request.Name,
                request.Claims);

            _roleClaimAggregationRepository.AddRoleClaims(role).GetAwaiter().GetResult();

            return new HandleResponse() { Content = role, ContentIdentification = role.Id };
        }
    }
}
