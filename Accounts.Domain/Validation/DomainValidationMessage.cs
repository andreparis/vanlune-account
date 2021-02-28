using Accounts.Domain.Enums;

namespace Accounts.Domain.Validation
{
    public class DomainValidationMessage
    {

        public DomainValidationMessage(ValidationLevel level, string message, params object[] messageParams) : this(level, null, message, messageParams) { }

        public DomainValidationMessage(ValidationLevel level, string message, string property, params object[] messageParams)
        {
            if (messageParams.Length > 0)
                message = string.Format(message, messageParams);

            this.Message = message;
            this.Level = level;
            this.Property = property;
        }

        public ValidationLevel Level { get; private set; }
        public string Property { get; }
        public string Message { get; private set; }
    }
}
