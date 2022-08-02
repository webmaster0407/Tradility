using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Tradility.Abstractions;
using Tradility.Abstractions.Services;
using Tradility.Common.Extensions;
using Tradility.Common.Localization;
using Tradility.Data;
using Tradility.Data.Extensions;
using Tradility.UI.Utils;
using Tradility.ViewModels;
using Windows.ApplicationModel.Store;
using Windows.Services.Store;

namespace Tradility.Windows.Settings
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler? Closed;

        public List<LanguageViewModel> Languages { get; set; }
        public LanguageViewModel SelectedLanguage { get; set; }
        private Language currentLanguage;
        public bool LanguageChanged => SelectedLanguage.Language != currentLanguage;

        public List<CurrencyViewModel> Currencies { get; set; }
        public CurrencyViewModel SelectedCurrency { get; set; }

        public ObservableCollection<TwsConnectionInfo> ConnectionInfos { get; } = new();

        public bool IsLoggingEnabled { get; set; }
        public string LogPath { get; set; }

        public ICommand SaveCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand AddConnectionCommand { get; }
        public ICommand DeleteConnectionCommand { get; }
        public ICommand EditConnectionCommand { get; }
        public ICommand SelectLogsDirectoryCommand { get; }
        public ICommand OpenLogsDirectoryCommand { get; }
        public ICommand ResetLogsDirectoryCommand { get; }
        public ICommand RemoveLogsCommand { get; }
        public ICommand CheckLicenseCommand { get; }

        public SettingsViewModel()
        {
            Currencies = new()
            {
                new(Currency.USD),
                new(Currency.EUR),
                new(Currency.GBP)
            };

            Languages = new()
            {
                new(Language.English),
                new(Language.German)
            };

            var languageCode = Properties.Settings.Default.Language;
            var languageVm = Languages.First(x => x.Language.ToCode() == languageCode);
            SelectedLanguage = languageVm;
            currentLanguage = SelectedLanguage.Language;

            var currencyCode = Properties.Settings.Default.Currency;
            var currencyVm = Currencies.First(x => x.Currency.ToString() == currencyCode);
            SelectedCurrency = currencyVm;

            SaveCommand = new DelegateCommand(Save);
            CloseCommand = new DelegateCommand(Close);

            AddConnectionCommand = new DelegateCommand(AddConnection);
            EditConnectionCommand = new DelegateCommand<TwsConnectionInfo>(EditConnection);
            DeleteConnectionCommand = new DelegateCommand<TwsConnectionInfo>(DeleteConnection);

            ConnectionInfos.AddRange(TwsConfigurationViewModel.Instance.Connections);

            IsLoggingEnabled = Properties.Settings.Default.EnableLogging;
            LogPath = Properties.Settings.Default.LoggingPath;

            SelectLogsDirectoryCommand = new DelegateCommand(SelectLogsDirectory);
            OpenLogsDirectoryCommand = new DelegateCommand(OpenLogsDirectory);
            ResetLogsDirectoryCommand = new DelegateCommand(ResetLogsDirectory);
            RemoveLogsCommand = new DelegateCommand(RemoveLogs);
            CheckLicenseCommand = new DelegateCommand(CheckLicense);
        }

        private async void CheckLicense()
        {
            var context = StoreContext.GetDefault();
            
            var licenseInformation = CurrentApp.LicenseInformation;
            var appLicense = await context.GetAppLicenseAsync();

            var aJson = new
            {
                licenseInformation.IsTrial,
                licenseInformation.IsActive,
                licenseInformation.ExpirationDate
            }.ToJson();
            var bJson = appLicense.ToJson();

            MessageBox.Show(aJson);
            MessageBox.Show(bJson);
        }

        private void RemoveLogs()
        {
            var dirInfo = new DirectoryInfo(LogPath);
            if (dirInfo.Exists)
            {
                dirInfo.Delete(true);
                dirInfo.Create();
            }
            MessageBox.Show("Logs cleared");
        }

        private void ResetLogsDirectory()
        {
            var newDir = Path.Combine(Path.GetTempPath(), "Tradility", "Logs");
            Directory.CreateDirectory(newDir);
            LogPath = newDir;
        }

        private void OpenLogsDirectory()
        {
            Directory.CreateDirectory(LogPath);
            Process.Start("explorer.exe", LogPath);
        }

        private void SelectLogsDirectory()
        {
            using var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "Select a folder to store log files",
                UseDescriptionForTitle = true,
                SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + Path.DirectorySeparatorChar,
                ShowNewFolderButton = true
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LogPath = dialog.SelectedPath;
            }
        }

        private void DeleteConnection(TwsConnectionInfo info)
        {
            if (info == TwsConfigurationViewModel.Instance.ActiveConnection)
            {
                MessageBox.Show("You can not delete active connection");
                return;
            }

            ConnectionInfos.Remove(info);
        }

        private void EditConnection(TwsConnectionInfo info)
        {
            if (info == TwsConfigurationViewModel.Instance.ActiveConnection)
            {
                MessageBox.Show("You can not edit active connection");
                return;
            }

            var dialog = new TwsConnectionWindow(info);

            if (dialog.ShowDialog() == true && dialog.Result is not null)
            {
                var index = ConnectionInfos.IndexOf(info);
                ConnectionInfos.RemoveAt(index);
                ConnectionInfos.Insert(index, dialog.Result);
            }
        }

        private void AddConnection()
        {
            var dialog = new TwsConnectionWindow(default);

            if (dialog.ShowDialog() == true && dialog.Result is not null)
            {
                ConnectionInfos.Add(dialog.Result);
            }
        }

        private void Save()
        {
            Properties.Settings.Default.Language = SelectedLanguage.Language.ToCode();
            Properties.Settings.Default.Currency = SelectedCurrency.Currency.ToString();

            Properties.Settings.Default.EnableLogging = IsLoggingEnabled;
            Properties.Settings.Default.LoggingPath = LogPath;

            Properties.Settings.Default.Save();

            IOC.Instance.Resolve<ILoggerOptions>().Udate(IsLoggingEnabled, LogPath);

            TwsConfigurationViewModel.Instance.Connections.ReplaceWithRange(ConnectionInfos);
            TwsConfigurationViewModel.Instance.SaveChanges();


            Close();
        }

        private void Close()
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }
    }    
}
