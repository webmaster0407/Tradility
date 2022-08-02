using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tradility.Data.Models;

namespace Tradility.Forms.Positions
{
    public class PositionColumns : ColumnsBase
    {
        public Column<PositionModel, string> Contract { get; set; } 
        public Column<PositionModel, string> Account { get; init; } 
        public Column<PositionModel, double> Pos { get; init; }  
        public Column<PositionModel, double> AvgCost { get; init; }

        public PositionColumns()
        {
            Contract = new("Contract", x => x.Contract.Symbol + " " + x.Contract.SecType + " " + x.Contract.Currency + " @ " + x.Contract.Exchange);
            Account = new("Account", x => x.Account);
            Pos = new("Position", x => x.Pos);
            AvgCost =  new("Average Cost", x => x.AvgCost);

            Columns = new()
            {
                Contract,
                Account,
                Pos,
                AvgCost
            };

            InitOrders();
        }
    }
}
