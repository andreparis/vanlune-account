using Accounts.Application.MediatR.Base;
using Accounts.Domain.DataAccess.Repositories;
using Accounts.Domain.DataAccess.Repositories.Aggregation;
using Accounts.Domain.Messaging;
using Accounts.Domain.Messaging.Email;
using Accounts.Domain.Security;
using Accounts.Infraestructure.Security;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Accounts.Application.Application.MediatR.Commands.CreateProducts
{
    public class CreateAccountsCommandHandler : AbstractRequestHandler<CreateAccountsCommand>
    {
        private readonly IRoleClaimAggregationRepository _roleClaimAggregationRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IAccountRepository _accountRepository;
        private IPasswordHasher _passwordHasher;
        private ISnsClient _snsClient;
        private IConfiguration _configuration;
        private ISecurityTokenHandler _securityTokenHandler;
        private readonly IAwsSecretManagerService _awsSecretManagerService;

        public CreateAccountsCommandHandler(IRoleClaimAggregationRepository roleClaimAggregationRepository,
            IRoleRepository roleRepository,
            IAccountRepository accountRepository,
            IPasswordHasher passwordHasher,
            ISnsClient snsClient,
            IConfiguration configuration,
            ISecurityTokenHandler securityTokenHandler,
            IAwsSecretManagerService awsSecretManagerService)
        {
            _roleClaimAggregationRepository = roleClaimAggregationRepository;
            _roleRepository = roleRepository;
            _accountRepository = accountRepository;
            _passwordHasher = passwordHasher;
            _snsClient = snsClient;
            _configuration = configuration;
            _securityTokenHandler = securityTokenHandler;
            _awsSecretManagerService = awsSecretManagerService;
        }

        internal override HandleResponse HandleIt(CreateAccountsCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return new HandleResponse()
                {
                    Error = "Email cannot be empty"
                };
            }

            var existingUser = _accountRepository
                .GetAccountByEmail(request.Email.Trim().ToLowerInvariant())
                .Result;

            if (existingUser != null)
            {
                return new HandleResponse() 
                {
                    Content = existingUser.IsActive ? "" : "Account pending validation.",
                    Error = $"E-mail already in use."
                };
            }

            AddUser(request).GetAwaiter().GetResult();

            SendConfirmationEmail(request.Email, request.Name).GetAwaiter().GetResult();

            return new HandleResponse() { Content = "Ready to buy!" };
        }

        private async Task AddUser(CreateAccountsCommand request)
        {
            var user = new Domain.Entities.User();
            var passwordHash = _passwordHasher.HashPassword(request.Password);
            request.Password = passwordHash;

            var roles = request.IdRoles.Count() == 0 ?
                new List<Domain.Entities.Role>()
                {
                    await _roleRepository
                    .GetRoleIdByName("Clients")
                    .ConfigureAwait(false)
                } :            
                await _roleRepository
                .GetRolesByIdAsync(request.IdRoles.ToArray())
                .ConfigureAwait(false);

            user.Create(
                request,
                roles);
            user.Email = request.Email.Trim().ToLowerInvariant();
            user.IsActive = false;

            await _roleClaimAggregationRepository.AddUserRoles(user).ConfigureAwait(false);
        }

        private async Task SendConfirmationEmail(string email, string userName)
        {
            var expires = DateTime.Now.AddDays(30);
            var issuer = _configuration["Issuer"];
            var securityKey = _awsSecretManagerService.GetSecret(_configuration["SecretName"]);
            var claims = new[]
            {
                    new System.Security.Claims.Claim(JwtRegisteredClaimNames.Sub, email),
                    new System.Security.Claims.Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                    new System.Security.Claims.Claim(JwtRegisteredClaimNames.UniqueName, email)
            };
            var serializedToken = _securityTokenHandler.WriteToken(email, claims, issuer, securityKey, expires);
            var url = _configuration["PLAYERS2_URL"] + "/confirm?u19=";
            var template = CreateAccountTemplate.GetAcocuntsBody(url + serializedToken);
            var emailTopic = _configuration["EMAIL_TOPIC"];
            var message = new Message()
            {
                Body = template,
                To = email,
                Subject = $"Welcome to PLAYER2 {userName}! Please confirm your e-mail here"
            };
            await _snsClient.Send(emailTopic, JsonConvert.SerializeObject(new
            {
                Message = message
            })).ConfigureAwait(false);
        }
    }
}
