using System;
using System.Globalization;
using Tradility.Common.Exceptions;
using Tradility.Common.Extensions;
using Tradility.Data.Models;

namespace Tradility.Data.Extensions
{
    public static class CurrencyExtensions
    {
        public static Currency Default => Currency.EUR;

        public static Currency ToCurrency(this string? currencyString, bool throwIfNull = false)
        {
            if (currencyString == null)
            {
                if (throwIfNull)
                    throw new ArgumentNullException(nameof(currencyString));

                return Default;
            }

            if (currencyString.Compare("EUR") || currencyString.Compare("EURO"))
                return Currency.EUR;
            else if (currencyString.Compare("USD"))
                return Currency.USD;
            else if (currencyString.Compare("GBP"))
                return Currency.GBP;
            else if (currencyString.Compare("CHF"))
                return Currency.CHF;
            else
                return Default;//throw new TException("Incorrect Currency");
        }

        public static Currency ToCurrency(this CultureInfo cultureInfo, bool throwException = false) => cultureInfo.Name switch
        {
            "de-DE" => Currency.EUR,
            "en-US" => Currency.USD,
            "en-GB" => Currency.GBP,
            "de-CH" => Currency.CHF,
            _ => throwException ? throw new TException("Incorrect Currency") : Default
        };

        public static CultureInfo ToCultureInfo(this Currency currency) => currency switch
        {
            Currency.EUR => new CultureInfo("de-DE"),
            Currency.USD => new CultureInfo("en-US"),
            Currency.GBP => new CultureInfo("en-GB"),
            Currency.CHF => new CultureInfo("de-CH"),
            _ => throw new NotImplementedException()
        };

        public static string ToMoney(this decimal value, Currency currency) => value.ToString("c", currency.ToCultureInfo());
        public static string ToMoney(this double value, Currency currency) => value.ToString("c", currency.ToCultureInfo());
        public static string ToMoney(this Currency currency, decimal value) => value.ToMoney(currency);

        public static bool Is(this Currency a, Currency b) => a.HasFlag(b);
        public static bool Is(this int a, Currency b) => ((Currency)a).HasFlag(b);
        public static bool IsUSD(this int a) => a.Is(Currency.USD);
        public static bool IsGBP(this int a) => a.Is(Currency.GBP);
        public static bool IsEUR(this int a) => a.Is(Currency.EUR);
        public static bool IsCHF(this int a) => a.Is(Currency.CHF);

        public static SymbolModel ToSymbol(this Currency currency) => new()
        {
            Symbol = currency.ToString(),
            Exchange = "IDEALPRO",
            SymbolType = SymbolType.Cash
        };
    }
}
