using System;

namespace Tradility.Data.Models
{
    public class CashReportModel : BaseModel
    {
        public string CurrencySummary { get; init; } = string.Empty;
        public string Currency { get; init; } = string.Empty;
        public decimal Total { get; init; }
        public decimal Securities { get; init; }
        public decimal Futures { get; init; }

        public override int GetHashCode() => HashCode.Combine(CurrencySummary, Currency, Total, Securities, Futures);
    }
}
