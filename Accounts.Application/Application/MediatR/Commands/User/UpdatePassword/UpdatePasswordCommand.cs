using Accounts.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Application.Application.MediatR.Commands.User.UpdatePassword
{
    public class UpdatePasswordCommand : IRequest<Response>
    {
        public int UserId { get; set; }
        public string Password { get; set; }
        public string NewPassowrd { get; set; }
    }
}
