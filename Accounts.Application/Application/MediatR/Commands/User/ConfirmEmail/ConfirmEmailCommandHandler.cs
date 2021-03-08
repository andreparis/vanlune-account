using Accounts.Application.MediatR.Base;
using Accounts.Domain.DataAccess.Repositories;
using Accounts.Domain.Security;
using Accounts.Infraestructure.Logging;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading;

namespace Accounts.Application.Application.MediatR.Commands.User.ConfirmEmail
{
    public class ConfirmEmailCommandHandler : AbstractRequestHandler<ConfirmEmailCommand>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger _logger;

        public ConfirmEmailCommandHandler(IAccountRepository accountRepository,
            ILogger logger)
        {
            _accountRepository = accountRepository;
            _logger = logger;
        }

        internal override HandleResponse HandleIt(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Link))
                {
                    return new HandleResponse()
                    {
                        Error = "Invalid email!"
                    };
                }

                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(request.Link);
                var tokenS = handler.ReadToken(request.Link) as JwtSecurityToken;
                var email = tokenS
                            .Claims
                            .First(claim => claim.Type == JwtRegisteredClaimNames.UniqueName)
                            .Value;

                var user = _accountRepository.GetAccountByEmail(email).Result;

                if (user == null)
                    return new HandleResponse()
                    {
                        Error = "Varification failed. Your e-mail is not valid!"
                    };

                user.IsActive = true;

                _accountRepository.UpdateAccount(user).GetAwaiter().GetResult();

                return new HandleResponse() { Content = user.Email };
            }
            catch (Exception e)
            {
                _logger.Error($"Error {e.Message} at {e.StackTrace}");

                return new HandleResponse()
                {
                    Error = "Error to activate user"
                };
            }
        }
    }
}
