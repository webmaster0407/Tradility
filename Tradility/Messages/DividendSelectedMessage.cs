using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tradility.Data.Models;

namespace Tradility.Messages
{
    public class DividendSelectedMessage
    {
        public DividendModel SelectedDividend { get; set; }

        public DividendSelectedMessage(DividendModel dividend)
        {
            SelectedDividend = dividend;
        }
    }
}
