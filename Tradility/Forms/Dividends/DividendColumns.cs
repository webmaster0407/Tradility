using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tradility.Data;
using Tradility.Data.Models;

namespace Tradility.Forms.Dividends
{
    public class DividendColumns : ColumnsBase
    {
        public Column<DividendModel, Currency> CurrencyColumn { get; init; }
        public Column<DividendModel, DateOnly> Date { get; init; }
        public Column<DividendModel, string> Description { get; init; }
        public Column<DividendModel, string> Amount { get; init; }
        public Column<DividendModel, string> AmountExchanged { get; init; }
        public Column<DividendModel, string> TickerSymbol { get; init; }
        public Column<DividendModel, string> ISIN { get; init; }
        public Column<DividendModel, decimal> DividendPerShare { get; init; }
        public Column<DividendModel, string> PaymentType { get; init; }

        public DividendColumns()
        {
            CurrencyColumn = new (R.Currency, x => x.Currency);
            Date = new(R.Date, x => x.Date);
            Description = new(R.Description, x => x.Description);
            Amount = new(R.Amount, x => x.AmountFormatted);
            AmountExchanged = new(x => $"{R.Amount} [{x.Currency}]", x => x.AmountExchanged);
            TickerSymbol = new("Ticker symbol", x => x.TickerSymbol);
            ISIN = new("ISIN", x => x.ISIN);
            DividendPerShare = new("Dividend/Share", x => x.DividendPerShare);
            PaymentType = new(R.PaymentType, x => x.PaymentType);

            Columns = new()
            {
                Date,
                TickerSymbol,
                ISIN,
                PaymentType,
                DividendPerShare,
                CurrencyColumn,
                Amount,
                AmountExchanged,
                Description
            };

            InitOrders();
        }
    }
}
