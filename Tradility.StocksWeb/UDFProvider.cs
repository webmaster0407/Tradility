using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tradility.Common.Extensions;
using Tradility.Data.Extensions;
using Tradility.Data.Services;
using Tradility.TWS;
using Tradility.TWS.API;
using Tradility.TWS.Requests;
using TradingViewUdfProvider;
using TradingViewUdfProvider.Models;

namespace Tradility.StocksWeb
{
    public class UDFProvider : ITradingViewProvider
    {
        private readonly object reqIdLocker;
        private static int reqId;
        private readonly Client twsClient;
        private readonly IMemoryCache cache;
        private readonly DumpService dump;

        public UDFProvider(Client twsClient, IMemoryCache cache, DumpService dumpService)
        {
            this.twsClient = twsClient;
            reqIdLocker = new object();
            reqId = 100;
            this.cache = cache;
            this.dump = dumpService;
        }

        public Task<TvConfiguration> GetConfiguration() => Task.FromResult(new TvConfiguration
        {
            SupportedResolutions = new[] {
                "D"
            },
            SupportGroupRequest = false,
            SupportMarks = false,
            SupportSearch = true,
            SupportTimeScaleMarks = false
        });

        private async Task<TvSymbolInfo> GetSymbolFromTWSAsync(string symbol)
        {
            await twsClient.ConnectIfNotAsync();     

            string? symbolName = null;
            string? symbolExchange = null;
            var splitted = symbol.Split(':');

            if (splitted.Length == 1)
            {
                symbolName = splitted[0].ToUpper();
            }
            else if (splitted.Length == 2)
            {
                symbolName = splitted[0].ToUpper();
                symbolExchange = splitted[1].ToUpper();
            }
            else
            {
                throw new Exception("Incorrect Symbol");
            }

            var matchRequest = new MatchingSymbolsRequest(twsClient);

            IEnumerable<ContractDescription> contractMatchs = await matchRequest.GetAsync(symbolName);

            if (!string.IsNullOrWhiteSpace(symbolExchange))
                contractMatchs = contractMatchs.Where(x => x.Contract?.Exchange?.ToUpper() == symbolExchange || x.Contract?.PrimaryExch?.ToUpper() == symbolExchange);
            contractMatchs = contractMatchs
                .Where(x => x.Contract?.Currency == "USD")
                .Where(x => x.Contract?.Symbol?.ToUpper() == symbolName);
            var contract = contractMatchs.FirstOrDefault()?.Contract;

            if (contract == null)
                throw new Exception("Symbol Not Found");

            var contractDetailsRequest = new ContractDetailsRequest(twsClient);
            var contractDetails = await contractDetailsRequest.GetAsync(contract);
            var contractDetail = contractDetails.FirstOrDefault();

            if (contractDetail == null)
                throw new Exception("Symbol Not Found");

            contract = contractDetail.Contract;
            cache.Set($"{symbol}:contract", contract);

            var response = new TvSymbolInfo
            {
                Name = contractDetail.LongName,
                Ticker = symbol,
                Type = "stock",
                HasDaily = true,
                CurrencyCode = "USD",
                ExchangeListed = contract.Exchange,
                ExchangeTraded = contract.Exchange
            };

            return response;

            //var contract = new Contract
            //{
            //    SecType = "STK",
            //    Currency = "USD"
            //};

            //if (splitted.Length == 1)
            //{
            //    contract.Symbol = splitted[0];
            //    contract.Exchange = "SMART";
            //}
            //else if (splitted.Length == 2)
            //{
            //    contract.Symbol = splitted[0];
            //    contract.Exchange = splitted[1] == "NASDAQ" ? "ISLAND" : splitted[1];

            //    if (contract.Exchange == "SMART")
            //        contract.PrimaryExch = "NASDAQ";
            //}
            //else
            //{
            //    throw new Exception("Incorrect Symbol");
            //}

            //try
            //{
            //    var contractDescriptions = await request.GetAsync(contract);               
            //    var contractDetail = contractDescriptions.FirstOrDefault();

            //    if (contractDetail == null)
            //        throw new Exception("Symbol Not Found");

            //    cache.Set($"{symbol}:contract", contractDetail);

            //    var response = new TvSymbolInfo
            //    {
            //        Name = contractDetail.LongName,
            //        Ticker = symbol,
            //        Type = "stock",
            //        HasDaily = true,
            //        CurrencyCode = "USD",
            //        ExchangeListed = contractDetail.Contract.Exchange,
            //        ExchangeTraded = contractDetail.Contract.Exchange
            //    };

            //    return response;
        //}
        //    catch (Exception)
        //    {
        //        throw new Exception("Incorrect Symbol");
        //    }
        }

        public async Task<TvSymbolInfo> GetSymbol(string symbol)
        {
            var symbolInfo = await cache.GetOrCreateAsync(symbol, (cacheEntry) =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);

                return GetSymbolFromTWSAsync(symbol);
            });

            return symbolInfo;
        }

        public Task<TvSymbolSearch[]> FindSymbols(string query, string type, string exchange, int? limit)
        {
            throw new NotImplementedException();
        }

        public async Task<TvBarResponse> GetHistory(DateTime from, DateTime to, string symbol, string resolution)
        {
            var symbolDataKey = $"{symbol}:data";
            var tvBars = await GetHistoryFromJson();
            //var tvBars = await cache.GetOrCreateAsync(symbolDataKey, cacheEntry =>
            //{
            //    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);

            //    return GetHistoryFromTWSAsync(symbol);
            //});

            var response = GetTvBarResponse(from, to, tvBars);

            return response;
        }

        public async Task<TvBar[]> GetHistoryFromTWSAsync(string symbol)
        {
            try
            {
                await twsClient.ConnectIfNotAsync();
                var request = new HistoricalDataRequest(twsClient);

                if (!cache.TryGetValue<TvSymbolInfo>(symbol, out var symbolInfo))
                    throw new Exception("Incorrect Symbol");

                var contract = cache.Get<Contract>($"{symbol}:contract");
                //var contract = new Contract
                //{
                //    Symbol = symbolSplitted[0],
                //    Currency = "USD",
                //    Exchange = symbolInfo.ExchangeTraded,
                //    SecType = "OPT"
                //};
                var bars = await request.GetAsync(contract, string.Empty, "5 Y", "1 day", "TRADES", 0, 1, false, new List<TagValue>());
                var tvBars = GetTvBars(bars);
                await dump.SaveAsync(symbol.Replace(":","-"), new
                {
                    bars,
                    tvBars
                });
                return tvBars;
            }
            catch (Exception ex)
            {
                await dump.SaveAsync($"Exception-{symbol.Replace(":", "-")}", ex.Message);
                throw;
            }
        }

        public async Task<TvBar[]> GetHistoryFromJson()
        {
            var file = await File.ReadAllTextAsync("bars.json");
            var bars = file.ToModel<List<Bar>>();
            var tvBars = GetTvBars(bars);          
            return tvBars;
        }

        public Task<TvMark[]> GetMarks(DateTime from, DateTime to, string symbol, string resolution)
        {
            throw new NotImplementedException();
        }

        private static TvBarResponse GetTvBarResponse(DateTime from, DateTime to, TvBar[] tvBars)
        {
            var foundBars = tvBars
                .Where(x => x.Timestamp >= from && x.Timestamp <= to)
                .OrderBy(x => x.Timestamp)
                .ToArray();
            var before = tvBars
                .OrderBy(x => x.Timestamp)
                .LastOrDefault(x => x.Timestamp < from);
            return new TvBarResponse()
            {
                Bars = foundBars,
                Status = foundBars.Any() ? TvBarStatus.Ok : TvBarStatus.NoData,
                NextTime = before?.Timestamp
            };
        }

        private static TvBar[] GetTvBars(List<Bar> bars)
        {
            var tvBars = bars!
               .Select(x => new TvBar
               {
                   Close = x.Close,
                   High = x.High,
                   Low = x.Low,
                   Open = x.Open,
                   Timestamp = x.Time.TWSDateTimeToDateTime(),
                   Volume = x.Volume
               })
               .OrderBy(x => x.Timestamp)
               .ToArray();
            return tvBars;
        }
    }
}
