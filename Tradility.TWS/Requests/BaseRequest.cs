using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;

namespace Tradility.TWS.Requests
{
    public abstract class BaseRequest
    {
        protected readonly Client Client;
        protected int ReqId;

        private readonly TaskCompletionSource taskCompletionSource;
        private readonly object isRunningLocker;
        private readonly System.Timers.Timer timer;
        private bool IsRunning;

        public BaseRequest(Client client)
        {
            Client = client;
            taskCompletionSource = new();

            timer = new System.Timers.Timer();
            timer.Elapsed += Timer_Elapsed;
            var timeoutTime = TimeSpan.FromSeconds(30).TotalMilliseconds;
            timer.Interval = timeoutTime;
            timer.AutoReset = false;

            isRunningLocker = new();

            Client.Wrapper.ConnectAck += OnConnectionAck;
            Client.Wrapper.ConnectionClosed += OnConnectionClosed;
            Client.Wrapper.Error += OnError;
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            taskCompletionSource.SetException(new Exception("TWS API Request Timeout"));
            EndRequest();
        }

        protected void TaskComplete()
        {
            taskCompletionSource.TrySetResult();
        }

        protected Task Task()
        {
            return taskCompletionSource.Task;
        }

        protected void StartRequest()
        {
            lock (isRunningLocker)
            {
                if (IsRunning)
                    throw new Exception("Request already working");

                IsRunning = true;
                ReqId = Client.GetRequestId();
                timer.Start();                
            }

            OnStartRequest();
        }

        protected void EndRequest()
        {
            lock (isRunningLocker)
            {
                IsRunning = false;
                timer.Stop();
            }

            OnEndRequest();
        }

        protected abstract void OnStartRequest();

        protected abstract void OnEndRequest();

        protected virtual void OnConnectionAck(object? sender, EventArgs e)
        {

        }

        protected virtual void OnConnectionClosed(object? sender, EventArgs e)
        {
            EndRequest();
        }

        protected virtual void OnError(object? sender, ErrorEventArgs e)
        {
            if (e.ErrorCode < 2100 || e.ErrorCode > 2169)
            {
                taskCompletionSource.TrySetException(new Exception(e.ErrorMsg));
                EndRequest();
            }

            else if (e.Exception != null)
            {
                taskCompletionSource.TrySetException(e.Exception);
                EndRequest();
            }

            Debug.WriteLine("LOG: " + e.ErrorMsg);
        }
    }
}
