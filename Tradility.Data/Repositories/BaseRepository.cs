using System.Collections.ObjectModel;
using System.ComponentModel;
using Tradility.Data.Models;
using Tradility.Data.Repositories.Abstractions;

namespace Tradility.Data.Repositories
{
    public abstract class BaseRepository<TModel> : IRepository, INotifyPropertyChanged where TModel : BaseModel 
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public virtual ObservableCollection<TModel> Items { get; protected set; } = new();

        public BaseRepository()
        {
            //Items = new();
        }
    }
}
