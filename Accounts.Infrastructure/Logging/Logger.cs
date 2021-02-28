using System;

namespace Accounts.Infraestructure.Logging
{
    public class Logger : ILogger
    {
        public void Debug(string message)
        {
#if DEBUG
            Console.WriteLine($"[DEBUG] {message}");
#endif
        }

        public void Error(string message)
        {
            Console.WriteLine($"[ERROR] {message}");
        }

        public void Info(string message)
        {
            Console.WriteLine($"[INFO] {message}");
        }

        public void Warning(string message)
        {
            Console.WriteLine($"[WARNING] {message}");
        }
    }
}
