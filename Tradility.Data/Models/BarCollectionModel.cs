using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tradility.Common.Extensions;
using Tradility.Data.Extensions;

namespace Tradility.Data.Models
{
    public class BarCollectionModel : BaseModel
    {
        public static readonly BarCollectionModel USDBarCollection;

        public SymbolModel Symbol { get; set; }
        public IDictionary<int, BarModel> Bars { get; init; }

        public BarCollectionModel(SymbolModel symbol, IEnumerable<BarModel> bars)
        {
            Symbol = symbol;
            Bars = bars.ToDictionary(x => x.Date.GetHashCode());
        } 

        static BarCollectionModel()
        {
            var symbol = Currency.USD.ToSymbol();
            var now = Now.DateOnly;
            var collection = Enumerable
                .Range(0, 1828)
                .OrderByDescending(x => x)
                .Select(x => new BarModel
                {
                    High = 1,
                    Open = 1,
                    Close = 1,
                    Low = 1,
                    Date = now.AddDays(-x)
                });

            USDBarCollection = new(symbol, collection);
        }
    }
}
