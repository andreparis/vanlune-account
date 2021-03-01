using Accounts.Application.Application.PatchHandlers;
using Accounts.Application.MediatR.Base;
using Accounts.Domain.DataAccess.Repositories.Aggregation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Accounts.Application.Application.MediatR.Commands.Role.RolePatch
{
    public class RolePatchCommandHandler : AbstractRequestHandler<RolePatchCommand>
    {
        private readonly IRoleClaimAggregationRepository _claimAggregationRepository;

        public RolePatchCommandHandler(IRoleClaimAggregationRepository claimAggregationRepository)
        {
            _claimAggregationRepository = claimAggregationRepository;
        }

        internal override HandleResponse HandleIt(RolePatchCommand request, CancellationToken cancellationToken)
        {
            var role = _claimAggregationRepository.GetRoleById(request.RoleId).Result;

            if (role == null)
                return new HandleResponse();

            // username replace
            var username = request.Patches.Where(e => e.Path == "/name" && e.Op == JsonPatchOperationType.REPLACE).FirstOrDefault()?.Value?.FirstOrDefault();

            role.Update(username);

            // claims assignment
            var claimsToBeAssignedTo = request.Patches.Where(e => e.Path == "/claims" && e.Op == JsonPatchOperationType.ADD).FirstOrDefault();
            var claimsToBeUnassignedFrom = request.Patches.Where(e => e.Path == "/claims" && e.Op == JsonPatchOperationType.REMOVE).FirstOrDefault();

            if (claimsToBeAssignedTo != null)
                foreach (var claimId in claimsToBeAssignedTo.Value.Select(e => int.Parse(e)))
                    role.AddClaimToRole (new Domain.Entities.Claim() { Id = claimId });

            if (claimsToBeUnassignedFrom != null)
                foreach (var claimId in claimsToBeUnassignedFrom.Value.Select(e => int.Parse(e)))
                    role.RemoveClaimFromRole(new Domain.Entities.Claim() { Id = claimId }
                    );

            _claimAggregationRepository.AttachClaimsToRole(role).GetAwaiter().GetResult();

            return new HandleResponse() { Content = true };
        }
    }
}
