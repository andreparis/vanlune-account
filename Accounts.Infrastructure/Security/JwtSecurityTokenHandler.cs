using Accounts.Domain.Security;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Accounts.Infrastructure.Security
{
    public class JwtSecurityTokenHandler : ISecurityTokenHandler
    {
        public string WriteToken(string username, IEnumerable<Claim> claims, string issuer, string securityKey, DateTime expires)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiresIn = expires.Subtract(DateTime.Now).Minutes * 60;

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: issuer,
                claims: claims,
                expires: expires,
                signingCredentials: signingCredentials);

            return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
