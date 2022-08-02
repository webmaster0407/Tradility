using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tradility.Data.Models
{
    public class SymbolModel : BaseModel
    {
        public string Symbol { get; init; } = string.Empty;
        public string Exchange { get; init; } = string.Empty;
        public SymbolType SymbolType { get; init; }

        public string SymbolTypeTWS => SymbolType switch
        {
            SymbolType.Stock => "STK",
            SymbolType.Cash => "CASH",
            _ => throw new NotImplementedException()
        };

        public override string ToString() => $"{Symbol} {Exchange} {SymbolType}";
        public override int GetHashCode() => HashCode.Combine(Symbol, Exchange, SymbolType);
        public override bool Equals(object? obj) => GetHashCode() == obj?.GetHashCode();
    }
}
