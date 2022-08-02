namespace Tradility.Abstractions.Services
{
    public interface ILoggerOptions
    {
        bool IsEnabled { get; }
        string DirectoryPath { get; }

        void Udate(bool isEnabled, string directoryPath);
    }

    public interface ILoggerFactory
    {
        ILogger CreateLogger(string message, string category);
    }

    public interface ILogger
    {
        string Category { get; }
        string? SubCategory { get; }
        string Message { get; }
        string[]? Extras { get; }

        ILogger WithSubCategory(string subCategory);
        ILogger WithExtras(params string[] extras);
        void Log();
    }
}
