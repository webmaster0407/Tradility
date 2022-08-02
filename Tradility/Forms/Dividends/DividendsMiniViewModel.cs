using PubSub;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Tradility.Data.Models;
using Tradility.Data.Repositories;
using Tradility.Messages;
using Tradility.UI.Utils;

namespace Tradility.Forms.Dividends
{
    public class DividendsMiniViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public DividendModel? SelectedItem { get; set; }
        public ICommand SelectedItemChangedCommand { get; set; }
        public ObservableCollection<DividendModel> Items { get; set; }

        private readonly DividendRepository repository;

        public DividendsMiniViewModel(DividendRepository repository)
        {
            this.repository = repository;
            this.repository.PropertyChanged += Repository_PropertyChanged;
            if (repository.Items != null)
                Items = new(repository.Items);

            SelectedItemChangedCommand = new DelegateCommand(SelectedItemsChanged);
            Hub.Default.Subscribe<StocksInitializedMessage>(StocksInitialized);
        }

        private void Repository_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender == repository && repository.Items != null && e.PropertyName == nameof(repository.Items))
            {
                Items = new(repository.Items);
            }
        }

        private async void SelectedItemsChanged()
        {
            if (SelectedItem != null)
                await PublishDividendSelected();
        }

        private async void StocksInitialized(StocksInitializedMessage message)
        {
            if (SelectedItem != null)
                await PublishDividendSelected();
        }

        private async Task PublishDividendSelected()
        {
            if (SelectedItem != null)
                await Hub.Default.PublishAsync(new DividendSelectedMessage(SelectedItem));
        }      
    }
}
