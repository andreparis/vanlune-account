using System.Collections.Generic;

namespace Accounts.Domain.Entities
{
    public class Response
    {
        public long Id { get; set; }
        public object Content { get; set; }
        public string Error { get; set; }
        public IEnumerable<Validation.DomainValidationMessage> DomainValidationMessages { get; set; }
    }
}
