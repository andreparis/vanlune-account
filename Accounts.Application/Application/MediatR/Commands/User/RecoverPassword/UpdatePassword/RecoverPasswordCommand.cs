using Accounts.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Application.Application.MediatR.Commands.User.RecoverPassword.UpdatePassword
{
    public class RecoverPasswordCommand : IRequest<Response>
    {
        public string Verification { get; set; }
        public string Password { get; set; }
    }
}
