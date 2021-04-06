using Accounts.Application.MediatR.Base;
using Accounts.Domain.DataAccess.Repositories;
using Accounts.Domain.DTO;
using AutoMapper;
using System.Collections.Generic;
using System.Threading;

namespace Accounts.Application.Application.MediatR.Commands.GetProducts
{
    public class GetAccountsbYfILTERSCommandHandler : AbstractRequestHandler<GetAccountsByFiltersCommand>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;

        public GetAccountsbYfILTERSCommandHandler(IAccountRepository accountRepository,
            IMapper mapper)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
        }

        internal override HandleResponse HandleIt(GetAccountsByFiltersCommand request, CancellationToken cancellationToken)
        {
            var accounts = _accountRepository.GetAccountsByFilters(request.Filters).Result;
            var dto = _mapper.Map<IEnumerable<UserDto>>(accounts);


            return new HandleResponse()
            {
                Content = dto
            };
        }
    }
}
