using Accounts.Application.MediatR.Base;
using Accounts.Domain.DataAccess.Repositories;
using Accounts.Domain.Security;
using Accounts.Infraestructure.Logging;
using Accounts.Infraestructure.Security;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading;

namespace Accounts.Application.Application.MediatR.Commands.User.AuthenticateUser
{
    public class AuthenticateUserCommandHandler : AbstractRequestHandler<AuthenticateUserCommand>
    {
        private IConfiguration _configuration;
        private ISecurityTokenHandler _securityTokenHandler;
        private IPasswordHasher _passwordHasher;
        private readonly IAccountRepository _accountRepository;
        private readonly IAwsSecretManagerService _awsSecretManagerService;
        private readonly ILogger _logger;

        public AuthenticateUserCommandHandler(IConfiguration configuration,
           ISecurityTokenHandler securityTokenHandler,
           IPasswordHasher passwordHasher,
           IAccountRepository accountRepository,
           IAwsSecretManagerService awsSecretManagerService,
           ILogger logger)
        {
            _configuration = configuration;
            _securityTokenHandler = securityTokenHandler;
            _passwordHasher = passwordHasher;
            _accountRepository = accountRepository;
            _awsSecretManagerService = awsSecretManagerService;
            _logger = logger;
        }

        internal override HandleResponse HandleIt(AuthenticateUserCommand request, CancellationToken cancellationToken)
        {
            _logger.Info($"Trying loggin for user {request.Email}");

            var user = _accountRepository.GetAccountByEmail(request.Email).Result;

            if (user == null)
            {
                _logger.Info($"User {request.Email} was not found on database.");
                return new HandleResponse() { Error = $"Invalid credentials, User {request.Email} was not found" };
            }
            var isPasswordValid = _passwordHasher.Verify(request.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                return new HandleResponse() { Error = $"Invalid credentials invalid password" };
            }
            var roleClaims = user.Roles.Select(e => e.Role.Claims.Select(c => c.Claim));

            var customClaims = new List<System.Security.Claims.Claim>();

            foreach (var roleClaim in roleClaims)
            {
                foreach (var claim in roleClaim)
                {
                    var customClaim = new System.Security.Claims.Claim(claim.ClaimType, claim.ClaimValue);
                    var existent = customClaims.FirstOrDefault(c => c.Type.Equals(customClaim.Type, StringComparison.InvariantCultureIgnoreCase) && c.Value.Equals(customClaim.Value, StringComparison.InvariantCultureIgnoreCase));
                    if (existent == null)
                        customClaims.Add(customClaim);
                }
            }

            var claims = new[]
            {
                    new System.Security.Claims.Claim(JwtRegisteredClaimNames.Sub, request.Email),
                    new System.Security.Claims.Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                    new System.Security.Claims.Claim(JwtRegisteredClaimNames.UniqueName, request.Email)
            }.Union(customClaims);

            var expiration = 60;
            var expires = DateTime.Now.AddMinutes(expiration);
            var issuer = _configuration["SecurityToken:Issuer"];
            var expires_in = expiration * 60;

            var securityKey = _awsSecretManagerService.GetSecret(_configuration["SecurityToken:SecretName"]);

            var serializedToken = _securityTokenHandler.WriteToken(request.Email, claims, issuer, securityKey, expires);

            return new HandleResponse()
            {
                Content = new
                {
                    access_token = serializedToken,
                    token_type = "Bearer",
                    expires_in
                }
            };
        }
    }
}
