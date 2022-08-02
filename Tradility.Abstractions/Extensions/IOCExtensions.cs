using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tradility.Abstractions.Services;
using Tradility.Abstractions.Services.Implementations;

namespace Tradility.Abstractions.Extensions
{
    public static class IOCExtensions
    {
        public static IOC AddLogger(this IOC ioc, bool isEnabled, string dirPath)
        {
            return ioc.RegisterInstance<ILoggerOptions, LoggerOptions>(new LoggerOptions(isEnabled, dirPath))
                .RegisterInstance<ILoggerFactory, LoggerFactory>(new LoggerFactory());
        }
    }
}
