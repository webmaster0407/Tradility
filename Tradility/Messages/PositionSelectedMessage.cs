using Tradility.Data.Models;

namespace Tradility.Messages
{
    public class PositionSelectedMessage
    {
        public PositionModel SelectedPosition { get; }

        public PositionSelectedMessage(PositionModel position)
        {
            SelectedPosition = position;
        }
    }
}
