using Accounts.Domain.Entities;
using MediatR;
using System;

namespace Accounts.Application.MediatR.Events
{
    public class IntegrationEvent : IRequest<Response>
    {
        public string EventName { get; set; }

        public object Message { get; set; }

        public DateTime PublishedAt { get; set; }
    }
}
