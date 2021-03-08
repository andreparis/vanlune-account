﻿using Accounts.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Application.Application.MediatR.Commands.User.ConfirmEmail
{
    public class ConfirmEmailCommand : IRequest<Response>
    {
        public string Link { get; set; }
    }
}
