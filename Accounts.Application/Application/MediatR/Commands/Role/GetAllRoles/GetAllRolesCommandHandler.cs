using Accounts.Application.MediatR.Base;
using Accounts.Domain.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Accounts.Application.Application.MediatR.Commands.Role.GetAllRoles
{
    public class GetAllRolesCommandHandler : AbstractRequestHandler<GetAllRolesCommand>
    {
        private readonly IRoleRepository _roleRepository;

        public GetAllRolesCommandHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        internal override HandleResponse HandleIt(GetAllRolesCommand request, CancellationToken cancellationToken)
        {
            var roles = _roleRepository.GetAllRoles().Result;

            return new HandleResponse() { Content = roles };
        }
    }
}
