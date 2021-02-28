using Accounts.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Accounts.Application.Application.MediatR.Commands.User.AuthenticateUser
{
    public class AuthenticateUserCommand : IRequest<Response>
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
