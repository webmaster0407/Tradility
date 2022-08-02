using PubSub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tradility.Data.Models;
using Tradility.Data.Repositories;
using Tradility.Messages;

namespace Tradility.Forms.Dividends
{
    public class DividendsViewModel : BaseRepositoryViewModel<DividendModel>
    {
        public DividendModel? SelectedItem { get; set; }
        public string? Currency { get; set; } = "";
        public DividendColumns? DividendColumns { get; set; }

        public DividendsViewModel(DividendRepository repository) : base(repository, new DividendColumns())
        {
            DividendColumns = Columns as DividendColumns;

            //Hub.Default.Subscribe<CurrencyChangedMessage>(CurrencyChanged);
        }

        private void CurrencyChanged(CurrencyChangedMessage message)
        {
            Currency = message.Currency.ToString();
            //DividendColumns.Currency = message.Currency;
        }
    }
}
