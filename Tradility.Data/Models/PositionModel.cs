using System;
using Tradility.TWS.API;

namespace Tradility.Data.Models
{
    public class PositionModel : BaseModel
    {
        public string Account { get; init; } = string.Empty;
        public double AvgCost { get; init; }
        public string ContractFormatted => Contract.Symbol + " " + Contract.SecType + " " + Contract.Currency + " @ " + Contract.Exchange; // TODO VM
        public Contract Contract { get; set; } 
        public double Pos { get; init; }
    }
}
