using System;
using System.ComponentModel;

namespace Tradility.TWS
{
    public class Settings : INotifyPropertyChanged
    {
        public static readonly Settings Instance = new();

        private string host = "localhost";
        public string Host
        {
            get => host;
            set
            {
                host = value;
                PropertyChanged?.Invoke(this, new(nameof(Host)));
            }
        }
        private int port = 7497;
        public int Port
        {
            get => port;
            set
            {
                port = value;
                PropertyChanged?.Invoke(this, new(nameof(Port)));
            }
        }
        private int clientId = 1;
        public int ClientID
        {
            get => clientId;
            set
            {
                clientId = value;
                PropertyChanged?.Invoke(this, new(nameof(ClientID)));
            }
        }

        private Settings()
        {

        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void ApplyConnectionInfo(string host, int port, int clientId)
        {
            Host = host;
            Port = port;
            ClientID = clientId;
        }

        public void ApplyConnectionInfo((string host, int port, int clientId) info) => ApplyConnectionInfo(info.host, info.port, info.clientId);
    }
}
