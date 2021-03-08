using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Domain.Messaging.Email
{
    public class Message
    {
        public string To { get; set; }
        public List<string> Bcs { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
    }
}
