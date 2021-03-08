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

namespace Accounts.Application.Application.MediatR.Commands.User.RecoverPassword.UpdatePassword
{
    public class RecoverPasswordCommandHandler : AbstractRequestHandler<RecoverPasswordCommand>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger _logger;

        public RecoverPasswordCommandHandler(IAccountRepository accountRepository,
            IPasswordHasher passwordHasher,
            ILogger logger)
        {
            _accountRepository = accountRepository;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        internal override HandleResponse HandleIt(RecoverPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Verification))
                {
                    return new HandleResponse()
                    {
                        Error = "Invalid link!"
                    };
                }

                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(request.Verification);
                var tokenS = handler.ReadToken(request.Verification) as JwtSecurityToken;
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
                if (tokenS.ValidTo < DateTime.Now)
                    return new HandleResponse()
                    {
                        Error = "Link expired. Please, try click forget password again!"
                    };
                if (string.IsNullOrEmpty(request.Password))
                    return new HandleResponse()
                    {
                        Content = "Link validated. Please inform a new password!"
                    };

                var passwordHash = _passwordHasher.HashPassword(request.Password);
                _accountRepository.UpdatePassword(user.Id, passwordHash).GetAwaiter().GetResult();

                return new HandleResponse()
                {
                    Content = "Password updated!"
                };
            }
            catch(Exception e)
            {
                _logger.Error($"Error {e.Message} at {e.StackTrace}");

                return new HandleResponse()
                {
                    Error = "Not possible update password. Please, try again later!"
                };
            }
        }
    }
}
