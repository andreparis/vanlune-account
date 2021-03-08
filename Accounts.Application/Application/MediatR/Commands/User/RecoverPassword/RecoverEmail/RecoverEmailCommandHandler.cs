using Accounts.Application.MediatR.Base;
using Accounts.Domain.DataAccess.Repositories;
using Accounts.Domain.Messaging;
using Accounts.Domain.Messaging.Email;
using Accounts.Domain.Security;
using Accounts.Infraestructure.Logging;
using Accounts.Infraestructure.Security;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Accounts.Application.Application.MediatR.Commands.User.RecoverPassword.RecoverEmail
{
    public class RecoverEmailCommandHandler : AbstractRequestHandler<RecoverEmailCommand>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ISnsClient _snsClient;
        private readonly IConfiguration _configuration;
        private readonly ISecurityTokenHandler _securityTokenHandler;
        private readonly IAwsSecretManagerService _awsSecretManagerService;
        private readonly ILogger _logger;

        public RecoverEmailCommandHandler(IAccountRepository accountRepository,
            ISnsClient snsClient,
            IConfiguration configuration,
            ISecurityTokenHandler securityTokenHandler,
            IAwsSecretManagerService awsSecretManagerService,
            ILogger logger)
        {
            _accountRepository = accountRepository;
            _snsClient = snsClient;
            _configuration = configuration;
            _securityTokenHandler = securityTokenHandler;
            _awsSecretManagerService = awsSecretManagerService;
            _logger = logger;
        }

        internal override HandleResponse HandleIt(RecoverEmailCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = _accountRepository.GetAccountByEmail(request.Email.ToLowerInvariant()).Result;

                if (user == null)
                {
                    return new HandleResponse()
                    {
                        Error = "E-mail not found!"
                    };
                }

                _logger.Info($"Trying recover password of user {JsonConvert.SerializeObject(user)}");

                SendRecoverEmail(user.Email, user.Name).GetAwaiter().GetResult();

                return new HandleResponse()
                {
                    Content = $"E-mail sent to {request.Email}"
                };
            }
            catch(Exception e)
            {
                _logger.Error($"Error {e.Message} at {e.StackTrace}");

                return new HandleResponse()
                {
                    Error = $"Failed to send recover e-mail. Try again later!"
                };
            }
        }


        private async Task SendRecoverEmail(string email, string userName)
        {
            var expires = DateTime.Now.AddHours(2);
            var issuer = _configuration["Issuer"];
            var securityKey = _awsSecretManagerService.GetSecret(_configuration["SecretName"]);
            var claims = new[]
            {
                    new System.Security.Claims.Claim(JwtRegisteredClaimNames.Sub, email),
                    new System.Security.Claims.Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                    new System.Security.Claims.Claim(JwtRegisteredClaimNames.UniqueName, email)
            };
            var serializedToken = _securityTokenHandler.WriteToken(email, claims, issuer, securityKey, expires);
            var url = _configuration["PLAYERS2_URL"] + "/newpassword?u17=";
            var template = ForgotPasswordTemplate.GetForgotPasswordBody(url + serializedToken);
            var emailTopic = _configuration["EMAIL_TOPIC"];
            var message = new Message()
            {
                Body = template,
                To = email,
                Subject = $"{userName}, change your password of PLAYER2 here!"
            };
            await _snsClient.Send(emailTopic, JsonConvert.SerializeObject(new
            {
                Message = message
            })).ConfigureAwait(false);
        }
    }
}
