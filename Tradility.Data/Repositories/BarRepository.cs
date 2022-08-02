using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tradility.Common.Exceptions;
using Tradility.Common.Extensions;
using Tradility.Common.Services;
using Tradility.Data.Extensions;
using Tradility.Data.Models;
using Tradility.TWS;
using Tradility.TWS.API;
using Tradility.TWS.Requests;

namespace Tradility.Data.Repositories
{
    public class BarRepository : TWSRepository<BarCollectionModel>
    {
        public override ObservableCollection<BarCollectionModel> Items
        {
            get => new(bars.Select(x => x.Value));
            protected set => bars = value.ToDictionary(x => x.Symbol, x => x);
        }

        private Dictionary<SymbolModel, BarCollectionModel> bars;
        private readonly CacheService cacheService;

        public BarRepository(Client twsClient, CacheService cacheService) : base(twsClient)
        {
            this.cacheService = cacheService;
            bars = new();

            var mainBarCollection = BarCollectionModel.USDBarCollection;
            bars[mainBarCollection.Symbol] = mainBarCollection;
        }

        public BarModel? GetBarModelByDate(SymbolModel symbol, DateOnly dateOnly)
        {
            BarModel? barModel = default;
            if (bars.TryGetValue(symbol, out var barCollectionModel))
                barCollectionModel.Bars.TryGetValue(dateOnly.GetHashCode(), out barModel);

            return barModel;
        }

        public BarModel? GetLatestBarModel(SymbolModel symbol)
        {
            BarModel? barModel = default;
            if (bars.TryGetValue(symbol, out var barCollectionModel))
                barModel = barCollectionModel.Bars.MaxBy(x => x.Key).Value;

            return barModel;
        }

        public BarModel? GetBarModel(SymbolModel symbol, DateOnly? date)
        {
            if (date == null)
                return GetLatestBarModel(symbol);
            else
                return GetBarModelByDate(symbol, date.Value);
        }

        public async Task LoadCurrenciesAsync()
        {
            await LoadAsync(Currency.EUR.ToSymbol());
            await LoadAsync(Currency.GBP.ToSymbol());
        }

        public async Task LoadAsync(SymbolModel symbol)
        {
            var key = GetKey(symbol);
            var cacheModel = await cacheService.TryGetOrCreateAsync(key, async () => await GetFromTWSAsync(symbol, 0));
            if (cacheModel == null)
                throw new TException($"Can't Load Symbol: {symbol}");
          
            var diffDays = GetTotalDaysFrom(cacheModel.Time);
            if (diffDays > 0)
            {
                if (diffDays > 365)
                {
                    cacheModel.Value = await GetFromTWSAsync(symbol, 0);
                }
                else
                {
                    var newItems = await GetFromTWSAsync(symbol, diffDays);
                    cacheModel.Value.AddRange(newItems);
                }

                await cacheService.TrySetAsync(key, cacheModel.Value);
            }


            var barsCollection = new BarCollectionModel(symbol, cacheModel.Value);
            bars[barsCollection.Symbol] = barsCollection;
        }

        private static int GetTotalDaysFrom(DateTimeOffset cacheTime)
        {
            var now = Now.DateTimeOffset;
            var diff = now.Date - cacheTime.Date;
            var totalDays = diff.TotalDays;
            var days = (int)Math.Round(totalDays);
            return days;
        }

        private static string GetKey(SymbolModel symbol) => symbol.Symbol + symbol.Exchange + symbol.SymbolType.ToString();

        private async Task<List<BarModel>> GetFromTWSAsync(SymbolModel symbol, int duration)
        {
            await TWSClient.ConnectAsync();
            var request = new HistoricalDataRequest(TWSClient);

            var contract = new Contract
            { 
                Symbol = symbol.Symbol,
                Currency = Currency.USD.ToString(),
                Exchange = symbol.Exchange,
                SecType = symbol.SymbolTypeTWS
            };

            var durationStr = duration == 0 ? "5 Y" : $"{duration} D";
            var twsBars = await request.GetAsync(contract, string.Empty, durationStr, "1 day", "MIDPOINT", 0, 1, false, new List<TagValue>());
            var bars = twsBars.Select(x => new BarModel
            {
                High = x.High,
                Open = x.Open,
                Close = x.Close,
                Low = x.Low,
                Date = x.Time.TWSDateTimeToDateOnly()
            })
            .ToList();

            return bars;
        }
    }
}
