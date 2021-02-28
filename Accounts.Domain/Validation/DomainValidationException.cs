using System.Collections.Generic;

namespace Accounts.Domain.Validation
{
    public class DomainValidationException : System.Exception
    {
        public DomainValidationException(IEnumerable<DomainValidationMessage> messages) : base()
        {
            this.ValidationErrors = messages;
        }

        public IEnumerable<DomainValidationMessage> ValidationErrors { get; }
    }
}
