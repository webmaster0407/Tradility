using PubSub;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Tradility.Data.Models;
using Tradility.Data.Repositories;
using Tradility.Messages;
using Tradility.UI.Utils;

namespace Tradility.Forms.Trades
{
    public class TradesViewModel : BaseRepositoryViewModel<TradeModel>
    {
        public TradeModel? SelectedItem { get; set; }
        public ICommand SelectedItemChangedCommand { get; set; }
        public string? Currency { get; set; } = "";
        public TradeColumns? TradeColumns { get; set; }

        public TradesViewModel(TradeRepository repository) : base(repository, new TradeColumns())
        {
            SelectedItemChangedCommand = new DelegateCommand(SelectedItemsChanged);
            Hub.Default.Subscribe<CurrencyChangedMessage>(CurrencyChanged);
            Hub.Default.Subscribe<StocksInitializedMessage>(StocksInitialized);

            TradeColumns = Columns as TradeColumns;
        }

        private async void SelectedItemsChanged()
        {
            //await PublishTradeSelected();
        }

        private async void StocksInitialized(StocksInitializedMessage message)
        {
            if (SelectedItem != null)
                await PublishTradeSelected();
        }

        private void CurrencyChanged(CurrencyChangedMessage message)
        {
            Currency = message.Currency.ToString();
        }

        private async Task PublishTradeSelected()
        {
            if (SelectedItem != null)
                await Hub.Default.PublishAsync(new TradeSelectedMessage(SelectedItem));
        }
    }
}
