using Accounts.Application.MediatR.Base;
using Accounts.Domain.DataAccess.Repositories;
using Accounts.Domain.DataAccess.Repositories.Aggregation;
using Accounts.Domain.Entities;
using Accounts.Domain.Security;
using System.Linq;
using System.Threading;

namespace Accounts.Application.Application.MediatR.Commands.CreateProducts
{
    public class CreateAccountsCommandHandler : AbstractRequestHandler<CreateAccountsCommand>
    {
        private readonly IRoleClaimAggregationRepository _roleClaimAggregationRepository;
        private readonly IRoleRepository _roleRepository;
        private IPasswordHasher _passwordHasher;

        public CreateAccountsCommandHandler(IRoleClaimAggregationRepository roleClaimAggregationRepository,
            IRoleRepository roleRepository,
             IPasswordHasher passwordHasher)
        {
            _roleClaimAggregationRepository = roleClaimAggregationRepository;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
        }

        internal override HandleResponse HandleIt(CreateAccountsCommand request, CancellationToken cancellationToken)
        {
            var user = new Domain.Entities.User();
            var passwordHash = _passwordHasher.HashPassword(request.Password);
            request.Password = passwordHash;

            var roles = _roleRepository.GetRolesByIdAsync(request.IdRoles.ToArray()).Result;

            user.Create(
                request,
                roles);

            _roleClaimAggregationRepository.AddUserRoles(user).GetAwaiter().GetResult();

            return new HandleResponse() { Content = user, ContentIdentification = user.Id };
        }
    }
}
