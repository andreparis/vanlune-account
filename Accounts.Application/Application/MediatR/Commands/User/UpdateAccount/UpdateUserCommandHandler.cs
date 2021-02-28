using Accounts.Application.MediatR.Base;
using Accounts.Domain.DataAccess.Repositories.Aggregation;
using Accounts.Domain.Entities;
using Accounts.Domain.Entities.CommandsInterface;
using Accounts.Domain.Security;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Accounts.Application.Application.MediatR.Commands.User.UpdateAccount
{
    public class UpdateUserCommandHandler : AbstractRequestHandler<UpdateUserCommand>
    {
        private readonly IRoleClaimAggregationRepository _roleClaimAggregationRepository;
        private IPasswordHasher _passwordHasher;

        public UpdateUserCommandHandler(IRoleClaimAggregationRepository roleClaimAggregationRepository, 
            IPasswordHasher passwordHasher)
        {
            _roleClaimAggregationRepository = roleClaimAggregationRepository;
            _passwordHasher = passwordHasher;
        }

        internal override HandleResponse HandleIt(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = _roleClaimAggregationRepository.GetUserById(request.Id).Result;

            if (user == null)
                return new HandleResponse();

            var passwordHash = _passwordHasher.HashPassword(request.Password);
            request.Password = passwordHash;

            //updates basic info for user
            user.Update(request);

            //Deassociate removed roles
            var rolesToRemove = user.GetJustTheRoles().Where(r => !request.IdRoles.Contains(r.Id));
            user.DesassociateRolesRange(rolesToRemove);

            _roleClaimAggregationRepository.UpdateAccountRoles(user).GetAwaiter().GetResult();

            return new HandleResponse() { Content = user, ContentIdentification = user.Id };
        }
    }
}
