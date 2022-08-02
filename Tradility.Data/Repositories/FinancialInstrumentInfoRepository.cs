using System;
using System.Linq;
using Tradility.Abstractions;
using Tradility.Abstractions.Services;
using Tradility.Common.Localization;
using Tradility.Data.Extensions;
using Tradility.Data.Models;

namespace Tradility.Data.Repositories
{
    public class FinancialInstrumentInfoRepository : CSVRepository<FinancialInstrumentInfoModel>
    {
        protected override string Key => TLocalization.Instance.Language switch
        {
            Language.English => "Financial Instrument Information",
            Language.German => "Informationen zum Finanzinstrument",
            _ => throw new NotImplementedException()
        };

        public FinancialInstrumentInfoModel? GetOrDefaultBySymbol(string symbol)
            => Items?.FirstOrDefault(x => x.Symbol == symbol && x.Category == AssetCategory.Stocks);

        protected override FinancialInstrumentInfoModel? Parse(Span<string> items)
        {
            try
            {
                var category = GetCategoryOrDefault(items.GetOrDefault(0));

                if (category == AssetCategory.Stocks)
                    return ParseStocks(items);
                else
                    return ParseEquityAndIndexOptions(items);
            }
            catch
            {
                return null;
            }
        }

        private static FinancialInstrumentInfoModel ParseStocks(Span<string> items)
        {
            var model = new FinancialInstrumentInfoModel
            {
                Category = GetCategoryOrDefault(items.GetOrDefault(0)),
                Symbol = items.GetOrDefault(1),
                Description = items.GetOrDefault(2),
                ConId = ParseDecimal(items.GetOrDefault(3)),
                SecurityId = items.GetOrDefault(4),
                ListingExchange = items.GetOrDefault(5),
                Multiplier = ParseDecimal(items.GetOrDefault(6)),
                Type = items.GetOrDefault(7),
            };

            return model;
        }

        private static FinancialInstrumentInfoModel ParseEquityAndIndexOptions(Span<string> items)
        {
            var model = new FinancialInstrumentInfoModel
            {
                Category = GetCategoryOrDefault(items.GetOrDefault(0)),
                Symbol = items.GetOrDefault(1),
                Description = items.GetOrDefault(2),
                ConId = ParseDecimal(items.GetOrDefault(3)),
                ListingExchange = items.GetOrDefault(4),
                Multiplier = ParseDecimal(items.GetOrDefault(5)),
                Expirity = ParseDate(items.GetOrDefault(6)),
                DeliveryMonth = ParseDateOnly(items.GetOrDefault(7) + "-01"),
                Type = items.GetOrDefault(8),
                Strike = ParseDecimal(items.GetOrDefault(9)),
            };          

            return model;
        }

        private static AssetCategory GetCategoryOrDefault(string assetCategory)
        {
            if (assetCategory == "Stocks" || assetCategory == "Aktien")
                return AssetCategory.Stocks;
            else if (assetCategory == "Equity and Index Options" || assetCategory == "Aktien- und Indexoptionen")
                return AssetCategory.EquityAndIndexOptions;
            else
            {
                var error = $"Asset category named '{assetCategory}' is not of known type.";
                IOC.Instance.Resolve<ILoggerFactory>().CreateLogger(error, "DataError").WithSubCategory("FinancialInstrumentInformation").Log();
                throw new ArgumentException(error, nameof(assetCategory));
            }
        }
    }
}
