using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tradility.Common.Extensions;
using Tradility.Data.Models;

namespace Tradility.Forms.CashReports
{
    public class CashReportColumns : ColumnsBase
    {
        public Column<CashReportModel, string> Currency { get; init; }
        public Column<CashReportModel, string> CurrencySummary { get; init; }
        public Column<CashReportModel, decimal> Securities { get; init; }
        public Column<CashReportModel, decimal> Futures { get; init; }
        public Column<CashReportModel, decimal> Total { get; init; }              

        public CashReportColumns()
        {
            Currency = new("Currency", x => x.Currency);
            CurrencySummary = new("Currency Summary", x => x.CurrencySummary);
            Securities = new("Securities", x => x.Securities);
            Futures = new("Futures", x => x.Futures);
            Total = new("Total", x => x.Total);

            Columns = new()
            {
                Currency,
                CurrencySummary,                             
                Securities,
                Futures,
                Total
            };

            InitOrders();
        }
    }
}
