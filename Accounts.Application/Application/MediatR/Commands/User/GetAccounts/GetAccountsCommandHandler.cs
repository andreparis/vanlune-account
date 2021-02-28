using Accounts.Application.MediatR.Base;
using System.Threading;

namespace Accounts.Application.Application.MediatR.Commands.GetProducts
{
    public class GetAccountsCommandHandler : AbstractRequestHandler<GetAccountsCommand>
    {
        internal override HandleResponse HandleIt(GetAccountsCommand request, CancellationToken cancellationToken)
        {
            return new HandleResponse()
            {
            };
        }
    }
}
