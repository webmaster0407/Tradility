using System;

namespace Tradility.Data.Models
{
    public class ExchangeRateModel : BaseModel
    {
        public DateOnly Date { get; init; }
        public decimal EUR { get; init; }
        public decimal USD { get; init; }
        public decimal GBP { get; init; }
        public decimal CHF { get; init; }
        public decimal GetRate(Currency currency) => currency switch
        {
            Currency.EUR => EUR,
            Currency.USD => USD,
            Currency.GBP => GBP,
            Currency.CHF => CHF,
            _ => throw new NotImplementedException(),
        };

        public decimal Convert(Currency from, Currency to, decimal value)
        {
            var fromRate = GetRate(from);
            var toRate = GetRate(to);

            if (value == 0 || fromRate == 0 || toRate == 0)
                return 0;

            var abs = value * fromRate;
            var result = abs / toRate;
            return result;
        }
    }
}
