using Accounts.Application.MediatR.Base;
using Accounts.Domain.DataAccess.Repositories;
using Accounts.Domain.DTO;
using Accounts.Domain.Entities;
using Accounts.Domain.Messaging;
using Accounts.Domain.Security;
using Accounts.Infraestructure.Logging;
using Accounts.Infraestructure.Security;
using Amazon;
using Amazon.Lambda;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Model;
using Amazon.Runtime;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _clientFactory;

        private const string LAMBDA_FACEBOOK_VALIDATED = "vanlune-accounts-int-authenticate";
        private const string LAMBDA_GOOGLE_VALIDATED = "vanlune-accounts-int-google";

        public AuthenticateUserCommandHandler(IConfiguration configuration,
           ISecurityTokenHandler securityTokenHandler,
           IPasswordHasher passwordHasher,
           IAccountRepository accountRepository,
           IAwsSecretManagerService awsSecretManagerService,
           ILogger logger,
           IMapper mapper,
           IHttpClientFactory clientFactory)
        {
            _configuration = configuration;
            _securityTokenHandler = securityTokenHandler;
            _passwordHasher = passwordHasher;
            _accountRepository = accountRepository;
            _awsSecretManagerService = awsSecretManagerService;
            _logger = logger;
            _mapper = mapper;
            _clientFactory = clientFactory;
        }

        internal override HandleResponse HandleIt(AuthenticateUserCommand request, CancellationToken cancellationToken)
        {
            var email = request.Email;

            _logger.Info($"Trying loggin for user {email}");
            
            var isFacebook = request.FacebookLogin != null && 
                !string.IsNullOrEmpty(request.FacebookLogin.Email) &&
                !string.IsNullOrEmpty(request.FacebookLogin.AccessToken);

            if (isFacebook)
            {
                var isValid = InvokeFunction(LAMBDA_FACEBOOK_VALIDATED, new { request.FacebookLogin }).Result;
                if (!isValid) return new HandleResponse() { Error = "Invalid token!" };
                email = request.FacebookLogin.Email;
            }

            var isGoogle = request.GoogleLogin != null &&
                !string.IsNullOrEmpty(request.GoogleLogin.AccessToken) &&
                 !string.IsNullOrEmpty(request.GoogleLogin.ProfileObj.Email);

            if (isGoogle)
            {
                var isValid = InvokeFunction(LAMBDA_GOOGLE_VALIDATED, new { request.GoogleLogin }).Result;
                if (!isValid) return new HandleResponse() { Error = "Invalid token!" };
                email = request.GoogleLogin.ProfileObj.Email;
            }

            var user = _accountRepository.GetAccountByEmail(email).Result;

            if (user == null)
            {
                _logger.Info($"User {email} was not found on database.");
                return new HandleResponse() { Error = $"Invalid credentials, User {email} was not found" };
            }

            if (!isFacebook && !isGoogle)
            {
                var isPasswordValid = _passwordHasher.Verify(request.Password, user.PasswordHash);

                if (!isPasswordValid)
                    return new HandleResponse() { Error = $"Invalid credentials invalid password" };
                if (!user.IsActive)
                    return new HandleResponse() { Error = "You must verificated your e-mai. Please, check your inbox messages." };
            }

            var roleClaims = user.Roles.Select(e => e.Role.Claims.Select(c => c.Claim));
            var customClaims = new List<System.Security.Claims.Claim>();

            foreach (var roleClaim in roleClaims)
            {
                foreach (var claim in roleClaim)
                {
                    var customClaim = new System.Security.Claims.Claim(claim.ClaimType, claim.ClaimValue);
                    var existent = customClaims.FirstOrDefault(c => c.Type.Equals(customClaim.Type, 
                        StringComparison.InvariantCultureIgnoreCase) && 
                        c.Value.Equals(customClaim.Value, StringComparison.InvariantCultureIgnoreCase));
                    if (existent == null)
                        customClaims.Add(customClaim);
                }
            }
            var roleId = user.Roles.FirstOrDefault()?.Role?.Id ?? 0;
            var claims = new[]
            {
                    new System.Security.Claims.Claim(JwtRegisteredClaimNames.Sub, roleId.ToString()),
                    new System.Security.Claims.Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                    new System.Security.Claims.Claim(JwtRegisteredClaimNames.UniqueName, email),
                    new System.Security.Claims.Claim(JwtRegisteredClaimNames.Sid, user.Id.ToString()),
            }.Union(customClaims);
            var expires = DateTime.Now.AddHours(2);
            var issuer = _configuration["Issuer"];
            var securityKey = _awsSecretManagerService.GetSecret(_configuration["SecretName"]);
            var serializedToken = _securityTokenHandler.WriteToken(email, claims, issuer, securityKey, expires);
            var userDto = _mapper.Map<UserDto>(user);

            return new HandleResponse()
            {
                Content = new
                {
                    access_token = serializedToken,
                    token_type = "Bearer",
                    user = userDto
                }
            };
        }

        private async Task<bool> InvokeFunction(string function, dynamic payload)
        {
            AmazonLambdaClient lambda;
            if (Debugger.IsAttached)
                lambda = new AmazonLambdaClient(new BasicAWSCredentials(_configuration["AWS:AccessKey"], _configuration["AWS:SecretKey"]), RegionEndpoint.USEast1);
            else
                lambda = new AmazonLambdaClient(RegionEndpoint.USEast1);
            var request = new InvokeRequest
            {
                FunctionName = function,
                Payload = JsonConvert.SerializeObject(new APIGatewayProxyRequest()
                {
                    Body = JsonConvert.SerializeObject(payload)
                })
            };
            var response = await lambda.InvokeAsync(request);

            using var sr = new StreamReader(response.Payload);
            var content = await sr.ReadToEndAsync();
            var result = JsonConvert.DeserializeObject<APIGatewayProxyRequest>(content);
            var fbResponse = JsonConvert.DeserializeObject<Response>(result.Body);
            return fbResponse.Content.Equals(true);
        }

    }
}
