using System.Threading.Tasks;
using Tradility.Abstractions;
using Tradility.Abstractions.Services;
using Tradility.Common.Extensions;

namespace Tradility.Data.Services
{
    public class DumpService
    {
        public Task SaveAsync<T>(string name, T value)
        {
            var ticks = Now.DateTimeOffset.Ticks;
            var json = value.ToJson(true);

            return Task.Run(() => IOC.Instance.Resolve<ILoggerFactory>().CreateLogger(json, name).WithSubCategory(ticks.ToString()).Log());
        }
    }
}
