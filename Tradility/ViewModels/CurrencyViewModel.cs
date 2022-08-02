using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tradility.Data;

namespace Tradility.ViewModels
{
    public class CurrencyViewModel
    {
        public Currency Currency { get; set; }
        public string Icon { get; set; }
        public string Name { get; set; }

        public CurrencyViewModel(Currency currency)
        {
            Currency = currency;
            Icon = $"pack://application:,,,/Properties/Icons/{currency}.png";
            Name = currency.ToString();
        }
    }
}
