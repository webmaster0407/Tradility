using System;
using System.Windows;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace Tradility.UI.Services
{
    public class Notifications
    {
        public static readonly Notifications Instance = new();
        private readonly Notifier notifier;

        public Notifications()
        {
            notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: Application.Current.MainWindow,
                    corner: Corner.BottomRight,
                    offsetX: 10,
                    offsetY: 10);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(3),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(5));

                cfg.Dispatcher = Application.Current.Dispatcher;
            });
        }

        public void Info(string message)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                notifier?.ShowInformation(message);
            }));
        }

        public void Warning(string message)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                notifier?.ShowWarning(message);
            }));
        }

        public void Error(string message)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                notifier?.ShowError(message);
            }));
        }

        public void Success(string message)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                notifier?.ShowSuccess(message);
            }));
        }
    }
}
