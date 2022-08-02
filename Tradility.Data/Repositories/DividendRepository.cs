using System;
using Tradility.Common.Extensions;
using Tradility.Common.Localization;
using Tradility.Data.Extensions;
using Tradility.Data.Models;
using Tradility.Data.Repositories.Abstractions;
using Tradility.Data.Services;

namespace Tradility.Data.Repositories
{
    public class DividendRepository : CSVRepository<DividendModel>, ICurrency
    {
        protected override string Key => TLocalization.Instance.Language switch
        {
            Language.English => "Dividends",
            Language.German => "Dividenden",
            _ => throw new NotImplementedException()
        };

        string TotalKey => TLocalization.Instance.Language switch
        {
            Language.English => "Total",
            Language.German => "Gesamt",
            _ => throw new NotImplementedException()
        };

        public Currency Currency { get; set; }

        private readonly ExchangeService exchangeService;

        public DividendRepository(ExchangeService exchangeService)
        {
            this.exchangeService = exchangeService;
        }

        protected override DividendModel? Parse(Span<string> items)
        {
            if (items.GetOrDefault(0).Contains(TotalKey, StringComparison.OrdinalIgnoreCase))
                return null;

            var item = new DividendModel
            {
                Currency = items.GetOrDefault(0).ToCurrency(),
                Date = ParseDividendDate(items.GetOrDefault(1)),
                Description = items.GetOrDefault(2),
                Amount = ParseDecimal(items.GetOrDefault(3))
            };
            item.ParseDescription();

            item.AmountFormatted = item.Amount.ToMoney(item.Currency);

            if (exchangeService.IsLoaded && item.Amount != 0)
            {
                var exchangeArg = new ExchangeService.ExchangeArgs
                {
                    Date = item.Date,
                    From = item.Currency,
                    To = Currency,
                    Value = item.Amount,
                };

                var exchanged = exchangeService.Exchange(exchangeArg);
                item.AmountExchanged = exchanged.ToMoney(Currency);
            }

            return item;
        }

        protected static DateOnly ParseDividendDate(string? content)
        {
            var cultureInfo = System.Globalization.CultureInfo.CurrentCulture;
            if (DateOnly.TryParseExact(content, "yyyy-MM-dd", out var date))
                return date;

            return default;
        }
    }
}
