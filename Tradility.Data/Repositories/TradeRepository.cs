using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Tradility.Common.Extensions;
using Tradility.Common.Localization;
using Tradility.Data.Extensions;
using Tradility.Data.Models;
using Tradility.Data.Repositories.Abstractions;
using Tradility.Data.Services;

namespace Tradility.Data.Repositories
{
    public class TradeRepository : CSVRepository<TradeModel>, ICurrency, INotifyPropertyChanged
    {
        protected override string Key => TLocalization.Instance.Language switch
        {
            Language.English => "Trades",
            Language.German => "Transaktionen",
            _ => throw new NotImplementedException()
        };

        public Currency Currency { get; set; }

        private readonly ExchangeService exchangeService;
        private readonly FinancialInstrumentInfoRepository financialInstrumentInfoRepository;

        public TradeRepository(ExchangeService exchangeService, FinancialInstrumentInfoRepository financialInstrumentInfoRepository)
        {
            this.exchangeService = exchangeService;
            this.financialInstrumentInfoRepository = financialInstrumentInfoRepository;
        }

        public override async Task LoadAsync(string filePath)
        {
            await financialInstrumentInfoRepository.LoadAsync(filePath);
            await base.LoadAsync(filePath);
        }

        protected override TradeModel Parse(Span<string> items)
        {
            var item = new TradeModel
            {
                DataDiscriminator = items.GetOrDefault(0),
                AssetCategory = items.GetOrDefault(1),
                Currency = items.GetOrDefault(2),
                Symbol = items.GetOrDefault(3),
                DateTime = ParseDate(items.GetOrDefault(4)),
                Quantity = ParseDecimal(items.GetOrDefault(5)),
                TPrice = ParseDecimal(items.GetOrDefault(6)),
                CPrice = ParseDecimal(items.GetOrDefault(7)),
                Proceeds = ParseDecimal(items.GetOrDefault(8)),
                CommFee = ParseDecimal(items.GetOrDefault(9)),
                Basis = ParseDecimal(items.GetOrDefault(10)),
                RealizedPL = ParseDecimal(items.GetOrDefault(11)),
                RealizedPLPercent = ParseDecimal(items.GetOrDefault(12)),
                MtmPL = ParseDecimal(items.GetOrDefault(13)),
                Code = items.GetOrDefault(14)
            };

            var currency = item.Currency!.ToCurrency(true);
            item.TPriceFormatted = item.TPrice.ToMoney(currency);
            item.CPriceFormatted = item.CPrice.ToMoney(currency);
            item.CommFeeFormatted = item.CommFee.ToMoney(currency);

            var financialInstrumentInfoModel = financialInstrumentInfoRepository.GetOrDefaultBySymbol(item.Symbol);
            item.Exchange = financialInstrumentInfoModel?.ListingExchange!;

            var exchangeArg = new ExchangeService.ExchangeArgs
            {
                Date = item.DateTime.ToDateOnly(), // TODO Check Timezones
                From = currency,
                To = Currency
            };

            if (exchangeService.IsLoaded)
            {
                exchangeArg.Value = 1;
                item.ExchangeRateFormatted = exchangeService.Exchange(exchangeArg)
                    .ToMoney(Currency);

                if (item.Proceeds != 0)
                {
                    exchangeArg.Value = item.Proceeds;
                    var exchanged = exchangeService.Exchange(exchangeArg);
                    item.ProceedsExchanged = exchanged.ToMoney(Currency);
                }

                if (item.TPrice > 0)
                {
                    exchangeArg.Value = item.TPrice;
                    var exchangedTPrice = exchangeService.Exchange(exchangeArg);
                    item.TPriceExchanged = exchangedTPrice.ToMoney(Currency);
                }

                if (item.CPrice > 0)
                {
                    exchangeArg.Value = item.CPrice;
                    var exchanged = exchangeService.Exchange(exchangeArg);
                    item.CPriceExchanged = exchanged.ToMoney(Currency);
                }

                if (item.CommFee != 0)
                {
                    exchangeArg.Value = item.CommFee;
                    var exchanged = exchangeService.Exchange(exchangeArg);
                    item.CommFeeExchanged = exchanged.ToMoney(Currency);
                }

                if (item.RealizedPL != 0)
                {
                    exchangeArg.Value = item.RealizedPL;
                    var exchanged = exchangeService.Exchange(exchangeArg);
                    item.RealizedPLExchanged = exchanged.ToMoney(Currency);
                }
            }            

            return item;
        }
    }
}
