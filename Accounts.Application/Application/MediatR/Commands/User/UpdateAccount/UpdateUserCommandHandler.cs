using Accounts.Application.MediatR.Base;
using Accounts.Domain.DataAccess.Repositories;
using Accounts.Domain.DataAccess.Repositories.Aggregation;
using Accounts.Domain.Entities;
using Accounts.Domain.Entities.CommandsInterface;
using Accounts.Domain.Security;
using Accounts.Infraestructure.Logging;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Accounts.Application.Application.MediatR.Commands.User.UpdateAccount
{
    public class UpdateUserCommandHandler : AbstractRequestHandler<UpdateUserCommand>
    {
        private readonly IAccountRepository _accountRepository;
        private IPasswordHasher _passwordHasher;
        private ILogger _logger;

        public UpdateUserCommandHandler(IAccountRepository accountRepository, 
            IPasswordHasher passwordHasher,
            ILogger logger)
        {
            _accountRepository = accountRepository;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        internal override HandleResponse HandleIt(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = _accountRepository.GetAccount(request.Id).Result;

                if (user == null)
                    return new HandleResponse()
                    {
                        Error = "User not exists"
                    };

                if (!string.IsNullOrEmpty(request.Password))
                {
                    var passwordHash = _passwordHasher.HashPassword(request.Password);
                    request.Password = passwordHash;
                }

                //updates basic info for user
                user.Update(request);

                _accountRepository.UpdateAccount(user).GetAwaiter().GetResult();

                _logger.Info($"Updated to {JsonConvert.SerializeObject(user)}");

                return new HandleResponse() { Content = user, ContentIdentification = user.Id };
            }
            catch(Exception e)
            {
                _logger.Error($"Error {e.Message} at {e.StackTrace}");

                return new HandleResponse()
                {
                    Error = "Not updated"
                };
            }
        }
    }
}
