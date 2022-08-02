using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tradility.Properties;

namespace Tradility.ViewModels
{
    public record TwsConnectionInfo(string DisplayName, string Host, int Port, int ClientId)
    {
        public (string host, int port, int clientId) GetInfo() => (Host, Port, ClientId);

        public override string ToString() => DisplayName;
    }
    public class TwsConfigurationViewModel : INotifyPropertyChanged
    {
        private static TwsConfigurationViewModel? _instance;
        public static TwsConfigurationViewModel Instance => _instance ??= (_instance = new TwsConfigurationViewModel());

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<TwsConnectionInfo> Connections { get; }

        private TwsConnectionInfo _activeConnection;

        public TwsConnectionInfo ActiveConnection
        {
            get => _activeConnection;
            set
            {
                _activeConnection = value;
                var twsSettings = TWS.Settings.Instance;
                twsSettings.ApplyConnectionInfo(value.GetInfo());
                SaveChanges();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ActiveConnection)));
            }
        }
        
        private TwsConfigurationViewModel()
        {
            Connections = new ObservableCollection<TwsConnectionInfo>();

            var json = Settings.Default.Connections;

            if (!string.IsNullOrWhiteSpace(json))
            {
                try
                {
                    var connections = System.Text.Json.JsonSerializer.Deserialize<TwsConnectionInfo[]>(json)!;
                    Connections.AddRange(connections);
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            if (!Connections.Any())
            {
                Connections.Add(GetDefaultConnection());
            }

            ActiveConnection = _activeConnection = Connections.ElementAtOrDefault(Settings.Default.ActiveConnectionId) ?? Connections.First();
        }

        private static TwsConnectionInfo GetDefaultConnection() => new("Default", "localhost", 7497, 1);

        public void SaveChanges()
        {
            Settings.Default.Connections = System.Text.Json.JsonSerializer.Serialize(Connections);
            Settings.Default.ActiveConnectionId = Connections.IndexOf(ActiveConnection);
            Settings.Default.Save();
        }

        public void Load() => Debug.WriteLine("Loaded");
    }
}
