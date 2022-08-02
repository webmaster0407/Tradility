using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tradility.Forms.SideBar
{
    /// <summary>
    /// Interaction logic for SideBarView.xaml
    /// </summary>
    public partial class SideBarView : UserControl
    {
        public SideBarView()
        {
            InitializeComponent();
        }

        private void NumTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Any(x => !Char.IsDigit(x)))
                e.Handled = true;
        }

        private void columnsLB_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var inputElement = e.OriginalSource as IInputElement;
            if (e.Delta < 0)
                ScrollBar.LineDownCommand.Execute(null, inputElement);
            if (e.Delta > 0)
                ScrollBar.LineUpCommand.Execute(null, inputElement);
            e.Handled = true;
        }
    }
}
