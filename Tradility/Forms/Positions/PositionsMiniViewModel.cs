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

namespace Tradility.Forms.Positions
{
    public class PositionsMiniViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public PositionModel? SelectedItem { get; set; }
        public ICommand SelectedItemChangedCommand { get; set; }

        public ObservableCollection<PositionModel> Items { get; set; }

        private readonly PositionRepository repository;

        public PositionsMiniViewModel(PositionRepository repository)
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

        private async void StocksInitialized(StocksInitializedMessage message)
        {
            if (SelectedItem != null)
                await PublishPositionSelected();
        }

        private async void SelectedItemsChanged()
        {
            if (SelectedItem != null)
                await PublishPositionSelected();
        }

        private async Task PublishPositionSelected()
        {
            if (SelectedItem != null)
                await Hub.Default.PublishAsync(new PositionSelectedMessage(SelectedItem));
        }
    }
}
