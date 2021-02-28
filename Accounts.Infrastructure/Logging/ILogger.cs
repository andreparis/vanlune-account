namespace Accounts.Infraestructure.Logging
{
    public interface ILogger
    {
        void Error(string message);
        void Info(string message);
        void Warning(string message);
        void Debug(string message);
    }
}
