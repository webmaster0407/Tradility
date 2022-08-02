using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tradility.Common.Extensions
{
    public static class IEnumerableExtensions
    {
        public static ObservableCollection<T>? ToObservableCollection<T>(this IEnumerable<T> collection, NotifyCollectionChangedEventHandler notifyCollectionChangedEventHandler = null)
        {
            if (collection == null)
                return null;

            var observableCollection = new ObservableCollection<T>(collection);
            if(notifyCollectionChangedEventHandler != null)
                observableCollection.CollectionChanged += notifyCollectionChangedEventHandler;
            return observableCollection;
        }
    }
}
