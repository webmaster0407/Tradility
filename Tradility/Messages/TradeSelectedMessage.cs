using Tradility.Data.Models;

namespace Tradility.Messages
{
    public class TradeSelectedMessage
    {
        public TradeModel SelectedTrade { get; }

        public TradeSelectedMessage(TradeModel selectedTrade)
        {
            SelectedTrade = selectedTrade;
        }
    }
}
