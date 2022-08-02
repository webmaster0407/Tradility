using GongSolutions.Wpf.DragDrop;
using Microsoft.Win32;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Tradility.Common.Exceptions;
using Tradility.Common.Extensions;
using Tradility.Data;
using Tradility.Data.Extensions;
using Tradility.Data.Repositories;
using Tradility.Data.Repositories.Abstractions;
using Tradility.Data.Services;
using Tradility.Forms.CashReports;
using Tradility.Forms.Dividends;
using Tradility.Forms.Positions;
using Tradility.Forms.Trades;
using Tradility.Messages;
using Tradility.Properties;
using Tradility.UI;
using Tradility.UI.Services;
using Tradility.UI.Utils;
using Tradility.ViewModels;

namespace Tradility.Forms.SideBar
{
    public class SideBarViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler? PositionsLoaded;

        public TWS.Settings TWSSettings { get; set; }

        public Currency[] Currencies { get; set; }
        public Currency SelectedCurrency { get; set; }

        public bool IsOnlyStocksRepositories { get; set; }
        [AlsoNotifyFor(nameof(Repositories))]
        private readonly List<TableViewModel> repositories;
        public IEnumerable<TableViewModel> Repositories => repositories.Where(x => !IsOnlyStocksRepositories || x.VisibleInStocks);
        public TableViewModel SelectedRepository { get; set; }

        [AlsoNotifyFor(nameof(FilePathIsNotEmpty))]
        public string? FilePath { get; set; }
        public bool FilePathIsNotEmpty => !string.IsNullOrWhiteSpace(FilePath);

        public bool IsLoading { get; set; }
        public bool IsLoadingCurrencies { get; set; }
        public bool IsLoadingPositions { get; set; }

        public ICommand ChooseFileCommand { get; set; }
        public ICommand LoadCurrenciesCommand { get; set; }
        public ICommand LoadPositionsCommand { get; set; }
        public ICommand LoadCommand { get; set; }

        private readonly ExchangeService exchangeService;
        private readonly PositionRepository positionRepository;

        public SideBarViewModel(TradesViewModel tradesViewModel,
            TradesMiniViewModel tradesMiniViewModel, 
            CashReportsViewModel cashReportsViewModel, 
            PositionsViewModel positionsViewModel, 
            PositionsMiniViewModel positionsMiniViewModel,
            DividendsViewModel dividendsViewModel,
            DividendsMiniViewModel dividendsMiniViewModel,
            ExchangeService exchangeService)
        {
            TWSSettings = TWS.Settings.Instance;

            Currencies = new[] {
                Currency.EUR,
                Currency.USD,
                Currency.GBP,
                Currency.CHF
            };

            SelectedCurrency = Settings.Default.Currency.ToCurrency();

            repositories = new()
            {
                new(Res.CashReport, cashReportsViewModel, null!, false),
                new(Res.Dividends, dividendsViewModel, dividendsMiniViewModel, true),
                new(Res.Positions, positionsViewModel, positionsMiniViewModel, true, false),
                new(Res.Trades, tradesViewModel, tradesMiniViewModel) // TODO VMs
            };
            SelectedRepository = Repositories.First();

            ChooseFileCommand = new DelegateCommand(ChooseFile);
            LoadCurrenciesCommand = new DelegateCommand(LoadCurrencies);
            LoadPositionsCommand = new DelegateCommand(LoadPositions);
            LoadCommand = new DelegateCommand(LoadAsync);

            this.exchangeService = exchangeService;
            this.positionRepository = (PositionRepository)positionsViewModel.Repository;
        }

        private void ChooseFile()
        {
            SafeCaller.Try(() =>
            {
                OpenFileDialog openFileDialog = new()
                {
                    Filter = "CSV|*.csv"
                };

                if (openFileDialog.ShowDialog() == true)
                    FilePath = openFileDialog.FileName;
            }, onError: _ => FilePath = null);

            LoadAsync();
        }

        private async void LoadCurrencies()
        {
            IsLoadingCurrencies = true;
            await SafeCaller.TryAsync(async () =>
            {
                SaveTWSSettings();
                await exchangeService.LoadAsync();
                Notifications.Instance.Success(R.CurrenciesLoadSuccess);
            });
            IsLoadingCurrencies = false;
        }

        private async void LoadPositions()
        {
            IsLoadingPositions = true;
            await SafeCaller.TryAsync(async () =>
            {
                SaveTWSSettings();
                await positionRepository.LoadAsync();
                Notifications.Instance.Success(R.PositionsLoadSuccess);
            });
            IsLoadingPositions = false;

            PositionsLoaded?.Invoke(this, EventArgs.Empty);
        }

        private async void LoadAsync()
        {
            IsLoading = true;

            await SafeCaller.TryAsync(async () =>
            {
                PubSub.Hub.Default.Publish(new CurrencyChangedMessage(SelectedCurrency));
                var selectedRepository = SelectedRepository?.Repository;
                var filePath = FilePath;

                if (selectedRepository is ICurrency repository)
                    repository.Currency = SelectedCurrency;

                if (selectedRepository is ILoadableRepository loadableRepository)
                    await loadableRepository.LoadAsync();

                if (selectedRepository is ICSVRepository csvRepository)
                {
                    //if (filePath == null)
                    //    throw new TException(Res.PleaseChooseFile);

                    if(!string.IsNullOrWhiteSpace(filePath))
                        await csvRepository.LoadAsync(filePath);
                }
            });

            IsLoading = false;
        }

        private void SaveTWSSettings()
        {
            //Settings.Default.TWSHost = TWSSettings.Host;
            //Settings.Default.TWSPort = TWSSettings.Port;
            //Settings.Default.TWSClientID = TWSSettings.ClientID;
            //Settings.Default.Save();
        }
    }
}
