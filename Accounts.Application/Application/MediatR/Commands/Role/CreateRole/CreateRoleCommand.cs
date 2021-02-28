using Accounts.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Accounts.Application.Application.MediatR.Commands.Role.CreateRole
{
    public class CreateRoleCommand : IRequest<Response>
    {
        public string Name { get; set; }
        [Required]
        public ICollection<int> Users { get; set; }
        public ICollection<int> Claims { get; set; }
    }
}
