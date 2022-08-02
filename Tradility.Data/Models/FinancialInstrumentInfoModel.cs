using System;

namespace Tradility.Data.Models
{
    public class FinancialInstrumentInfoModel : BaseModel
    {
        // Common
        public AssetCategory Category { get; init; }             // 0
        public string Symbol { get; init; } = string.Empty;      // 1
        public string Description { get; init; } = string.Empty; // 2
        public decimal ConId { get; init; } // 3

        public string ListingExchange { get; init; } = string.Empty; // if Stocks = 5 || EquityAndIndexOptions = 4
        public decimal Multiplier { get; init; }                     // if Stocks = 6 || EquityAndIndexOptions = 5
        public string Type { get; init; } = string.Empty;            // if Stocks = 7 || EquityAndIndexOptions = 8

        // Stocks
        public string SecurityId { get; init; } = string.Empty; // 4

        // EquityAndIndexOptions
        public DateTimeOffset Expirity { get; init; }  // 6
        public DateOnly DeliveryMonth { get; init; }   // 7
        public decimal Strike { get; init; }           // 9

        public override int GetHashCode()
        {
            var a = HashCode.Combine(Category, Symbol, Description, ConId);
            var b = HashCode.Combine(ListingExchange, Multiplier, Type);
            var c = HashCode.Combine(SecurityId);
            var d = HashCode.Combine(Expirity, DeliveryMonth, Strike);
            return HashCode.Combine(a, b, c, d);
        }
    }
}
