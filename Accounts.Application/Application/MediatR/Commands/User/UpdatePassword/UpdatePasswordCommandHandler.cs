using Accounts.Application.MediatR.Base;
using Accounts.Domain.DataAccess.Repositories;
using Accounts.Domain.Security;
using Accounts.Infraestructure.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Accounts.Application.Application.MediatR.Commands.User.UpdatePassword
{
    public class UpdatePasswordCommandHandler : AbstractRequestHandler<UpdatePasswordCommand>
    {
        private readonly IAccountRepository _accountRepository;
        private IPasswordHasher _passwordHasher;
        private ILogger _logger;

        public UpdatePasswordCommandHandler(IAccountRepository accountRepository,
            IPasswordHasher passwordHasher,
            ILogger logger)
        {
            _accountRepository = accountRepository;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        internal override HandleResponse HandleIt(UpdatePasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = _accountRepository.GetAccount(request.UserId).Result;
                if (user == null)
                    return new HandleResponse()
                    {
                        Error = "Invalid user!"
                    };

                var isValid = _passwordHasher.Verify(request.Password, user.PasswordHash);

                if (!isValid)
                    return new HandleResponse()
                    {
                        Error = "Invalid password!"
                    };

                var passwordHash = _passwordHasher.HashPassword(request.NewPassowrd);

                _accountRepository.UpdatePassword(request.UserId, passwordHash);

                return new HandleResponse()
                {
                    Content = "Passord updated!"
                };
            }
            catch(Exception e)
            {
                _logger.Error($"Error {e.Message} at {e.StackTrace}");

                return new HandleResponse()
                {
                    Error = "Passoword not updated. Please, try again later!"
                };
            }
        }
    }
}
