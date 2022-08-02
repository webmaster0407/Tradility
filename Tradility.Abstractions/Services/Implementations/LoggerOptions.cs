using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tradility.Abstractions.Services.Implementations
{
    internal class LoggerOptions : ILoggerOptions
    {
        public bool IsEnabled { get; private set; }

        public string DirectoryPath { get; private set; }

        public LoggerOptions(bool isEnabled, string directoryPath)
        {
            (IsEnabled, DirectoryPath) = (isEnabled, directoryPath);
        }

        public void Udate(bool isEnabled, string directoryPath) =>
            (IsEnabled, DirectoryPath) = (isEnabled, directoryPath);
    }
}
