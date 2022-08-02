using System;

namespace Tradility.Data.Models
{
    public class TradeModel : BaseModel
    {
        public string DataDiscriminator { get; init; } = string.Empty;
        public string AssetCategory { get; init; } = string.Empty;
        public string Currency { get; init; } = string.Empty;
        public string Symbol { get; init; } = string.Empty;
        public string Exchange { get; set; } = string.Empty;
        public DateTimeOffset DateTime { get; init; }
        public decimal Quantity { get; init; }
        public decimal TPrice { get; init; }
        public decimal CPrice { get; init; }
        public decimal Proceeds { get; init; }
        public decimal CommFee { get; init; }
        public decimal Basis { get; init; }
        public decimal RealizedPL { get; init; }
        public decimal RealizedPLPercent { get; init; }
        public decimal MtmPL { get; init; }
        public string Code { get; init; } = string.Empty;

        // TODO VM
        public string? TPriceFormatted { get; set; }
        public string? TPriceExchanged { get; set; }
        public string? CPriceFormatted { get; set; }
        public string? CPriceExchanged { get; set; }
        public string? CommFeeFormatted { get; set; }
        public string? CommFeeExchanged { get; set; }
        public string? ProceedsExchanged { get; set; }
        public string? RealizedPLExchanged { get; set; }
        public string? ExchangeRateFormatted { get; set; }

        public override int GetHashCode()
        {
            var a = HashCode.Combine(DataDiscriminator, AssetCategory, Currency, Symbol, Exchange, DateTime, Quantity, TPrice);
            var b = HashCode.Combine(CPrice, Proceeds, CommFee, Basis, RealizedPL, RealizedPLPercent, MtmPL, Code);
            return HashCode.Combine(a, b);
        }
    }
}
