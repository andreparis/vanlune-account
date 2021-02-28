using Accounts.Application.Application.PatchHandlers;
using Accounts.Application.MediatR.Base;
using Accounts.Domain.DataAccess.Repositories.Aggregation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Accounts.Application.Application.MediatR.Commands.User.UserPatch
{
    public class UserPatchCommandHandler : AbstractRequestHandler<UserPatchCommand>
    {
        private readonly IRoleClaimAggregationRepository _claimAggregationRepository;
        private IMediator _mediator;

        public UserPatchCommandHandler(IRoleClaimAggregationRepository claimAggregationRepository, 
            IMediator mediator)
        {
            _claimAggregationRepository = claimAggregationRepository;
            _mediator = mediator;
        }

        internal override HandleResponse HandleIt(UserPatchCommand request, CancellationToken cancellationToken)
        {
            var user = _claimAggregationRepository.GetUserById(request.UserId).Result;

            if (user == null)
                return new HandleResponse();

            var rolesToBeAssignedTo = request.Patches.Where(e => e.Path == "/roles" && e.Op == JsonPatchOperationType.ADD).FirstOrDefault();
            var rolesToBeUnassignedFrom = request.Patches.Where(e => e.Path == "/roles" && e.Op == JsonPatchOperationType.REMOVE).FirstOrDefault();

            if (rolesToBeAssignedTo != null)
                foreach (var roleId in rolesToBeAssignedTo.Value.Select(e => int.Parse(e)))
                    user.AddRoleToUser(new Domain.Entities.Role() { Id = roleId });

            if (rolesToBeUnassignedFrom != null)
                foreach (var roleId in rolesToBeUnassignedFrom.Value.Select(e => int.Parse(e)))
                    user.RemoveUserFromRole(roleId);

            _claimAggregationRepository.AttachRolesToUser(user).GetAwaiter().GetResult();

            return new HandleResponse() { Content = true };
        }
    }
}
