using Accounts.Application.MediatR.Base;
using Accounts.Domain.DataAccess.Repositories;
using System.Threading;

namespace Accounts.Application.Application.MediatR.Commands.Role.UpdateRole
{
    public class UpdateRoleCommandHandler : AbstractRequestHandler<UpdateRoleCommand>
    {
        private readonly IRoleRepository _roleRepository;

        public UpdateRoleCommandHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        internal override HandleResponse HandleIt(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            _roleRepository.UpdateAsync(request.Role).GetAwaiter().GetResult();

            return new HandleResponse() { Content = request.Role, ContentIdentification = request.Role.Id };
        }
    }
}
