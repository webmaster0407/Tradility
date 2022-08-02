namespace Tradility.Abstractions.Services.Implementations
{
    internal class LoggerFactory : ILoggerFactory
    {
        public ILogger CreateLogger(string message, string category) => new Logger(message, category);
    }
}
