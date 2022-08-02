using System;
using System.Threading.Tasks;
using Tradility.Common.Exceptions;
using Tradility.Common.Extensions;
using Tradility.Data.Extensions;
using Tradility.Data.Models;
using Tradility.Data.Repositories;

namespace Tradility.Data.Services
{
    public class ExchangeService
    {
        public bool IsLoaded { get; private set; }

        private readonly ExchangeRateRepository exchangeRateRepository;
        private readonly BarRepository barRepository;

        public ExchangeService(ExchangeRateRepository exchangeRateRepository, BarRepository barRepository)
        {
            this.exchangeRateRepository = exchangeRateRepository;
            this.barRepository = barRepository;
        }

        public async Task LoadAsync()
        {
            //if (IsLoaded)
            //return;
            await exchangeRateRepository.LoadAsync();
            //await barRepository.LoadCurrenciesAsync();
            IsLoaded = true;
        }

        public decimal Exchange(ExchangeArgs exchangeArgs)
        {
            if (exchangeArgs == null)
                throw new ArgumentNullException(nameof(exchangeArgs));

            var exchangeRate = exchangeRateRepository.GetByDate(exchangeArgs.Date) ?? exchangeRateRepository.GetLatest();

            if (exchangeRate == null)
                return 0;

            var result = exchangeRate.Convert(exchangeArgs.From, exchangeArgs.To, exchangeArgs.Value);

            return result;
        }

        public decimal Exchange(SymbolModel fromSymbol, SymbolModel toSymbol, decimal value, DateOnly? date)
        {
            var fromBar = barRepository.GetBarModel(fromSymbol, date);
            var toBar = barRepository.GetBarModel(toSymbol, date);

            if (fromBar == null || toBar == null)
                return 0;

            var exchangedValue = Convert(fromBar, toBar, value);

            return exchangedValue;
        }

        public decimal GetExchangeRate(Currency from, Currency to, DateOnly date)
        {
            var fromBar = barRepository.GetBarModelByDate(from.ToSymbol(), date);
            var toBar = barRepository.GetBarModelByDate(to.ToSymbol(), date);

            if (fromBar == null || toBar == null)
                return 0;

            var rate = Convert(fromBar, toBar, 1);
            
            return rate;
        }

        public decimal ExchangeByTrade(TradeModel tradeModel)
        {
            var from = tradeModel.Currency.ToCurrency().ToSymbol();
            var to = Currency.USD.ToSymbol();
            var date = tradeModel.DateTime.ToDateOnly();

            return Exchange(from, to, 1, date);
        }

        private static decimal Convert(BarModel fromBar, BarModel toBar, decimal value)
        {
            var fromRate = fromBar.AVG;
            var toRate = toBar.AVG;

            var abs = value * fromRate;
            var result = abs / toRate;
            return result;
        }

        

        public class ExchangeArgs
        {
            public Currency From { get; set; }
            public Currency To { get; set; }
            public decimal Value { get; set; }
            public DateOnly Date { get; set; }
        }
    }
}
