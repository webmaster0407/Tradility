using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Tradility.Common.Exceptions;
using Tradility.Common.Extensions;
using Tradility.Common.Services;
using Tradility.Data.Extensions;
using Tradility.Data.Models;
using Tradility.Data.Repositories.Abstractions;
using Tradility.TWS;
using Tradility.TWS.API;
using Tradility.TWS.Requests;

namespace Tradility.Data.Repositories
{
    public class ExchangeRateRepository : BaseRepository<ExchangeRateModel>, ILoadableRepository
    {
        const string ExchangeRatesCacheKey = "ExchangeRates";
        public const Currency MainCurrency = Currency.USD;

        public override ObservableCollection<ExchangeRateModel> Items
        {
            get => new(exchangeRates.Select(x => x.Value));
            protected set => exchangeRates = value.ToDictionary(x => x.Date.GetHashCode(), x => x);
        }

        private Dictionary<int, ExchangeRateModel> exchangeRates;
        private readonly Client client;
        private readonly CacheService cacheService;

        public ExchangeRateRepository(Client client, CacheService cacheService)
        {
            exchangeRates = new();
            
            this.client = client;
            this.cacheService = cacheService;
        }

        public async Task LoadAsync()
        {
            var cacheModel = await cacheService.TryGetOrCreateAsync(ExchangeRatesCacheKey, async () => await LoadFromTWSAsync());
            if (cacheModel == null)
                throw new TException("Can't Load Exchange Rates");

            var diffDays = IsNeedUpdate(cacheModel.Time);
            if (diffDays > 0)
            {
                if (diffDays > 365)
                {
                    cacheModel.Value = await LoadFromTWSAsync();
                }
                else
                {
                    var newItems = await LoadFromTWSAsync(diffDays);
                    foreach (var item in newItems)
                        cacheModel.Value[item.Key] = item.Value;
                }         

                await cacheService.TrySetAsync(ExchangeRatesCacheKey, cacheModel.Value);
            }

            exchangeRates = cacheModel.Value;
        }

        private int IsNeedUpdate(DateTimeOffset cacheTime)
        {
            var now = Now.DateTimeOffset;
            var diff = now.Date - cacheTime.Date;
            var totalDays = diff.TotalDays;
            var days = (int)Math.Round(totalDays);
            return days;
        }

        private async Task<Dictionary<int, ExchangeRateModel>> LoadFromTWSAsync(int duration = 0)
        {
            var items = new Dictionary<int, ExchangeRateModel>();
            await client.ConnectAsync();

            var eur = await LoadHistoricalData(Currency.EUR, duration);// await eurTask;
            var gbp = await LoadHistoricalData(Currency.GBP, duration);// await gbpTask;
            var chf = await LoadHistoricalData(Currency.CHF, duration);

            foreach (var (First, Second) in eur.Zip(gbp).Zip(chf))
            {
                var date = Second.Time.TWSDateTimeToDateOnly();
                var exchangeRateModel = new ExchangeRateModel
                {
                    Date = date,
                    USD = 1,
                    EUR = (decimal)(First.First.High + First.First.Low) / 2,
                    GBP = (decimal)(First.Second.High + First.Second.Low) / 2,
                    CHF = (decimal)(Second.High + Second.Low) / 2,
                };
                items[date.GetHashCode()] = exchangeRateModel;
            }

            return items;
        }

        public ExchangeRateModel? GetByDate(DateOnly date) => exchangeRates.TryGetValue(date.GetHashCode(), out var exchangeRate) ? exchangeRate : null;

        public ExchangeRateModel[] GetByDates(DateOnly[] dates) => exchangeRates
            .Join(dates,
                exchangeRate => exchangeRate.Value.Date,
                date => date,
                (exchangeRate, date) => exchangeRate.Value)
            .ToArray();

        public ExchangeRateModel? GetLatest() => exchangeRates.Select(x => x.Value).OrderByDescending(x => x.Date).FirstOrDefault();

        private Task<List<Bar>> LoadHistoricalData(Currency symbol, int duration)
        {
            //var reqId = (int)(Currency.USD | symbol);
            var request = new HistoricalDataRequest(client);

            var contract = new Contract
            {
                Symbol = symbol.ToString(),
                Currency = MainCurrency.ToString(),
                Exchange = "IDEALPRO",
                SecType = "CASH"
            };

            var durationStr = duration == 0 ? "5 Y" : $"{duration} D";

            //var endDate = $"{Now.DateOnly:yyyyMMdd} 23:59:59";
            return request.GetAsync(contract, String.Empty, durationStr, "1 day", "MIDPOINT", 0, 1, false, new List<TagValue>());
        }
    }
}
