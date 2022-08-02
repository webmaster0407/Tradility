using System;
using System.Threading;
using System.Threading.Tasks;
using Tradility.Common.Exceptions;
using Tradility.TWS.API;

namespace Tradility.TWS
{
    public class Client
    {
        private readonly EReaderMonitorSignal signal;
        private int nextValidId = 0;
        private TaskCompletionSource taskSource;
        private readonly System.Timers.Timer timer;

        public EClientSocket Socket { get; }
        public Wrapper Wrapper { get; }

        public Client()
        {
            signal = new();

            Wrapper = new();
            Wrapper.NextValidId += Wrapper_NextValidId;
            Wrapper.ManagedAccounts += Wrapper_ManagedAccounts;

            Socket = new(Wrapper, signal);

            //taskSource = new();
            timer = new();
            timer.Interval = TimeSpan.FromSeconds(30).TotalMilliseconds;
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = false;
        }

        /// <summary>
        /// Uses static Settings
        /// </summary>
        public Task ConnectAsync()
        {
            var twsSettings = Settings.Instance;
            return ConnectAsync(twsSettings.Host, twsSettings.Port, twsSettings.ClientID);
        }

        public async Task ConnectIfNotAsync()
        {
            if (!Socket.IsConnected())
                await ConnectAsync();
        }

        public async Task ConnectAsync(string host, int port, int clientId)
        {
            if (taskSource != null)
                await taskSource.Task;

            if (Socket.IsConnected())
                Disconnect();

            taskSource = new();
            timer.Start();
            await Task.Run(() => Connect(host, port, clientId));
            await taskSource.Task;
            timer.Stop();
            taskSource = null;
        }

        private void Connect(string host, int port, int clientId)
        {
            try
            {
                Socket.eConnect(host, port, clientId);
                var reader = new EReader(Socket, signal);

                reader.Start();

                var newThread = new Thread(() =>
                {
                    while (Socket.IsConnected())
                    {
                        signal.waitForSignal();
                        reader.processMsgs();
                    }
                });
                newThread.IsBackground = true;
                newThread.Start();
            }
            catch (Exception)
            {
                throw new TException("TWS Connection Exception");
            }
        }

        public void Disconnect()
        {
            if (Socket.IsConnected())
                Socket.eDisconnect();
        }

        private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            taskSource.TrySetException(new Exception("TWS Connect Timeout"));
            taskSource = null;
            timer.Stop();
        }

        private void Wrapper_ManagedAccounts(object? sender, ManagedAccountsEventArgs e)
        {
            taskSource?.TrySetResult();
        }

        private void Wrapper_NextValidId(object? sender, NextValidIdEventArgs e)
        {
            taskSource?.TrySetResult();
        }

        public int GetRequestId()
        {
            Interlocked.Increment(ref nextValidId);
            return nextValidId;
        }
    }
}
