using System.ComponentModel;
using Tradility.Data.Repositories.Abstractions;
using Tradility.Forms;

namespace Tradility.ViewModels
{
    public class TableViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public string Name { get; }
        public IRepository Repository { get; }
        public IRepositoryViewModel ViewModel { get; }
        public object MiniViewModel { get; set; }
        public bool VisibleInStocks { get; }
        public bool CanLoad { get; set; }

        public TableViewModel(string name, IRepositoryViewModel viewModel, object miniViewModel, bool visibleInStocks = true, bool canLoad = true)
        {
            Name = name;
            Repository = viewModel.Repository;
            ViewModel = viewModel;
            MiniViewModel = miniViewModel;
            VisibleInStocks = visibleInStocks;
            CanLoad = canLoad;
        }

        public override string ToString() => Name;
    }
}
