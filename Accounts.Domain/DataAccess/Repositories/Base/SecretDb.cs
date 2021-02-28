namespace Accounts.Domain.DataAccess.Repositories.Base
{
    public class SecretDb
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string DbInstanceIdentifier { get; set; }
    }
}
