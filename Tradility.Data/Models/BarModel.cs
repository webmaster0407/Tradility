using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tradility.Data.Models
{
    public class BarModel : BaseModel
    {
        public double High { get; init; }
        public double Open { get; init; }
        public double Close { get; init; }
        public double Low { get; init; }
        public DateOnly Date { get; init; }

        public decimal AVG => ((decimal)High + (decimal)Low) / 2;

        public override int GetHashCode() => HashCode.Combine(High, Open, Close, Low);
    }
}
