using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tradility.Forms.Positions
{
    /// <summary>
    /// Interaction logic for PositionsMiniView.xaml
    /// </summary>
    public partial class PositionsMiniView : UserControl
    {
        public PositionsMiniView()
        {
            InitializeComponent();

            dataGrid.RowHeight = double.NaN;
        }
    }
}
