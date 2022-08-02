using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tradility.Extensions
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this ObservableCollection<T> data, IEnumerable<T> toAdd)
        {
            foreach (var item in toAdd)
            {
                data.Add(item);
            }
        }

        public static void ReplaceWithRange<T>(this ObservableCollection<T> data, IEnumerable<T> toAdd)
        {
            data.Clear();
            data.AddRange(toAdd);
        }
    }
}
