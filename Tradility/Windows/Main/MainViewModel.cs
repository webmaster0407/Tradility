using System.ComponentModel;
using Tradility.Data.Repositories;
using Tradility.Forms.Stocks;
using Tradility.Forms.Tables;

namespace Tradility.Windows.Main
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public TablesViewModel TablesViewModel { get; set; }
        public StocksViewModel StocksViewModel { get; set; }

        public MainViewModel(TablesViewModel tablesViewModel, StocksViewModel stocksViewModel)
        {
            //TablesViewModel = new TablesViewModel(new TradeRepository(), new CashReportRepository(), new PositionsRepository());
            //StocksViewModel = new StocksViewModel();
            TablesViewModel = tablesViewModel;
            StocksViewModel = stocksViewModel;
        }
    }
}
