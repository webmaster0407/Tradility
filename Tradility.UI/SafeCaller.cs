using System;
using System.Threading.Tasks;
using Tradility.Abstractions;
using Tradility.Abstractions.Services;
using Tradility.UI.Services;

namespace Tradility.UI
{
    public static class SafeCaller
    {
        public static void Try(Action action, Action<Exception>? onError = null)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex);
                HandleError(ex);
            }
        }

        public static async Task TryAsync(Func<Task> action, Action<Exception>? onError = null)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex);
                HandleError(ex);
            }
        }

        private static void HandleError(Exception ex)
        {
            Notifications.Instance.Error(ex.Message);
            IOC.Instance.Resolve<ILoggerFactory>().CreateLogger(ex.ToString(), "Errors").Log();
        }
    }
}
