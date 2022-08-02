using Tradility.Data;

namespace Tradility.Messages
{
    public class CurrencyChangedMessage
    {
        public Currency Currency { get; set; }

        public CurrencyChangedMessage(Currency currency)
        {
            Currency = currency;
        }
    }
}
