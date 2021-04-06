using MediatR;
using Accounts.Domain.Entities;
using System.Collections.Generic;

namespace Accounts.Application.Application.MediatR.Commands.GetProducts
{
    public class GetAccountsByFiltersCommand : IRequest<Response>
    {
        public IDictionary<string, string> Filters { get; set; }
    }
}
