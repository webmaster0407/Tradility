using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Tradility.Data;
using Tradility.Data.Models;

namespace Tradility.Forms.Trades
{
    public class TradeColumns : ColumnsBase
    {
        public Column<TradeModel, string> DataDiscriminator { get; init; }
        public Column<TradeModel, string> AssetCategory { get; init; }
        public Column<TradeModel, string> Currency { get; init; }
        public Column<TradeModel, string> Symbol { get; init; }
        public Column<TradeModel, DateTimeOffset> DateTime { get; init; }
        public Column<TradeModel, decimal> Quantity { get; init; }
        public Column<TradeModel, decimal> TPrice { get; init; }
        public Column<TradeModel, string?> TPriceExchanged { get; set; }
        public Column<TradeModel, decimal> CPrice { get; init; }
        public Column<TradeModel, string?> CPriceExchanged { get; set; }
        public Column<TradeModel, decimal> Proceeds { get; init; }
        public Column<TradeModel, string?> ProceedsExchanged { get; set; }
        public Column<TradeModel, decimal> CommFee { get; init; }
        public Column<TradeModel, string?> CommFeeExchanged { get; set; }
        public Column<TradeModel, decimal> RealizedPL { get; init; }
        public Column<TradeModel, string?> RealizedPLExchanged { get; set; }
        public Column<TradeModel, decimal> RealizedPLPercent { get; init; }
        public Column<TradeModel, decimal> MtmPL { get; init; }
        public Column<TradeModel, string> Code { get; init; }
        public Column<TradeModel, string?> ExchangeRate { get; set; }

        public TradeColumns()
        {
            DataDiscriminator = new("Data Discriminator", x => x.DataDiscriminator);
            AssetCategory = new("Asset Category", x => x.AssetCategory);
            Currency = new("Currency", x => x.Currency);
            Symbol = new("Symbol", x => x.Symbol);
            DateTime = new("DateTime", x => x.DateTime);
            Quantity = new("Quantity", x => x.Quantity);
            TPrice = new("T. Price", x => x.TPrice);
            TPriceExchanged = new(x => $"T. Price [{x.Currency}]", x => x.TPriceExchanged);
            CPrice = new("C. Price", x => x.CPrice);
            CPriceExchanged = new(x => $"C. Price [{x.Currency}]", x => x.CPriceExchanged);
            Proceeds = new("Proceeds", x => x.Proceeds);
            ProceedsExchanged = new(x => $"Proceeds [{x.Currency}]", x => x.ProceedsExchanged);
            CommFee = new("Comm/Fee", x => x.CommFee);
            CommFeeExchanged = new(x => $"Comm/Fee [{x.Currency}]", x => x.CommFeeExchanged);
            RealizedPL = new("Realized P/L", x => x.RealizedPL);
            RealizedPLExchanged = new(x => $"Realized P/L [{x.Currency}]", x => x.RealizedPLExchanged);
            RealizedPLPercent = new("Realized P/L %", x => x.RealizedPLPercent);
            MtmPL = new("MTM P/L", x => x.MtmPL);
            Code = new("Code", x => x.Code);
            ExchangeRate = new("Exchange Rate", x => x.ExchangeRateFormatted);

            Columns = new()
            {
                DataDiscriminator,
                AssetCategory,
                Currency,
                Symbol,
                DateTime,
                Quantity,
                TPrice,
                TPriceExchanged,
                CPrice,
                CPriceExchanged,
                Proceeds,
                ProceedsExchanged,
                CommFee,
                CommFeeExchanged,
                RealizedPL,
                RealizedPLExchanged,
                RealizedPLPercent,
                MtmPL,
                Code,
                ExchangeRate,
            };

            InitOrders();
        }
    }
}
