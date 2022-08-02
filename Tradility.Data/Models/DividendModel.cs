using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tradility.Data.Models
{
    public class DividendModel : BaseModel
    {
        public Currency Currency { get; set; }
        public DateOnly Date { get; set; }
        public string Description { get; set; } = "";
        public decimal Amount { get; set; }

        public string TickerSymbol { get; set; } = "";
        public string ISIN { get; set; } = "";
        public decimal DividendPerShare { get; set; }
        public string PaymentType { get; set; } = "";

        // VM
        public string AmountFormatted { get; set; } = "";
        public string AmountExchanged { get; set; } 

        public void ParseDescription()
        {
            if (string.IsNullOrWhiteSpace(Description))
                return;

            var indexOfBracketStart = Description.IndexOf('(');
            var indexOfBracketEnd = Description.IndexOf(')');

            if (indexOfBracketEnd <= indexOfBracketStart)
                return;

            TickerSymbol = Description[..indexOfBracketStart];
            ISIN = Description.Substring(indexOfBracketStart + 1, indexOfBracketEnd - indexOfBracketStart - 1);

            PaymentType = Description[(indexOfBracketEnd + 1)..].Trim();

            int numberIndexStart = -1;
            int numberIndexEnd = -1;
            
            for (int i = 0; i < PaymentType.Length; i++)
            {
                var character = PaymentType[i];

                if (char.IsDigit(character) || (numberIndexStart >= 0 && (character == '.' || character == ',')))
                {
                    if(numberIndexStart < 0)
                    {
                        numberIndexStart = i;
                    }
                    numberIndexEnd = i;
                }
                else if(numberIndexStart >= 0)
                {
                    break;
                }
            }

            if(numberIndexStart >= 0)
            {
                var amount = PaymentType.Substring(numberIndexStart, numberIndexEnd - numberIndexStart + 1);
                DividendPerShare = decimal.Parse(amount.Replace(',', '.'), CultureInfo.InvariantCulture);
                PaymentType = string.Join(' ', PaymentType.Substring(0, numberIndexStart).Split(' ', StringSplitOptions.RemoveEmptyEntries).SkipLast(1));
            }
            else
            {
                PaymentType = PaymentType.Split('(').First();
            }

        }
    }
}
