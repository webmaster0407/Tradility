using PubSub;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Tradility.Common.Extensions;
using Tradility.Data.Models;
using Tradility.Data.Repositories;
using Tradility.Messages;
using Tradility.UI.Utils;

namespace Tradility.Forms.Trades
{
    public class TradesMiniViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ItemViewModel? SelectedItem { get; set; }
        public ICommand SelectedItemChangedCommand { get; set; }
        public ObservableCollection<ItemViewModel>? Items { get; set; }

        private readonly TradeRepository repository;

        public TradesMiniViewModel(TradeRepository repository)
        {
            this.repository = repository;
            this.repository.PropertyChanged += Repository_PropertyChanged;
            InitItems();

            SelectedItemChangedCommand = new DelegateCommand(SelectedItemsChanged);
            Hub.Default.Subscribe<StocksInitializedMessage>(StocksInitialized);
        }

        private void InitItems()
        {
            Items = repository.Items
                .GroupBy(x => x.Symbol)
                .Select(x => new ItemViewModel(x.OrderByDescending(x => x.DateTime).First()))
                .ToObservableCollection();                
        }

        private void Repository_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender == repository && repository.Items != null && e.PropertyName == nameof(repository.Items))
            {
                InitItems();
            }
        }

        private async void SelectedItemsChanged()
        {
            await PublishTradeSelected();
        }

        private async void StocksInitialized(StocksInitializedMessage message)
        {
            if (SelectedItem != null)
                await PublishTradeSelected();
        }

        private async Task PublishTradeSelected()
        {
            if (SelectedItem != null)
                await Hub.Default.PublishAsync(new TradeSelectedMessage(SelectedItem.TradeModel));
        }

        public class ItemViewModel
        {
            public string Symbol { get; set; }
            public string Value { get; set; }
            public TradeModel TradeModel { get; set; }

            public ItemViewModel(TradeModel tradeModel)
            {
                TradeModel = tradeModel;
                Symbol = tradeModel.Symbol;
                Value = tradeModel.TPriceExchanged ?? tradeModel.TPriceFormatted;
            }
        }
    }
}
