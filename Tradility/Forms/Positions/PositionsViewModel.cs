using PubSub;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Tradility.Data;
using Tradility.Data.Models;
using Tradility.Data.Repositories;
using Tradility.Data.Repositories.Abstractions;
using Tradility.Messages;
using Tradility.UI.Utils;

namespace Tradility.Forms.Positions
{
    public class PositionsViewModel : BaseRepositoryViewModel<PositionModel>, ICurrency
    {
        public Currency Currency { get; set; }
        public PositionModel? SelectedItem { get; set; }
        public ICommand SelectedItemChangedCommand { get; set; }
        public PositionColumns PositionColumns { get; set; }

        public PositionsViewModel(PositionRepository repository) : base(repository, new PositionColumns())
        {
            SelectedItemChangedCommand = new DelegateCommand(SelectedItemsChanged);
            SelectedItem = Items?.FirstOrDefault();

            PositionColumns = Columns as PositionColumns;

            Hub.Default.Subscribe<StocksInitializedMessage>(StocksInitialized);
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
