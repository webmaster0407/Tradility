using System;
using Tradility.Common.Localization;
using Tradility.Data.Extensions;
using Tradility.Data.Models;

namespace Tradility.Data.Repositories
{
    public class CashReportRepository : CSVRepository<CashReportModel>
    {
        protected override string Key => TLocalization.Instance.Language switch
        {
            Language.English => "Cash Report",
            Language.German => "Cash-Bericht",
            _ => throw new NotImplementedException()
        };

        protected override CashReportModel Parse(Span<string> items)
        {
            var item = new CashReportModel
            {
                CurrencySummary = items.GetOrDefault(0),
                Currency = items.GetOrDefault(1),
                Total = ParseDecimal(items.GetOrDefault(2)),
                Securities = ParseDecimal(items.GetOrDefault(3)),
                Futures = ParseDecimal(items.GetOrDefault(4))
            };

            return item;
        }
    }
}
