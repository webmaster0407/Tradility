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
using System.Windows.Shapes;

namespace Tradility.Windows.Settings
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(SettingsViewModel settingsViewModel)
        {
            InitializeComponent();

            DataContext = settingsViewModel;
            settingsViewModel.Closed += SettingsViewModel_Closed;
        }

        private void SettingsViewModel_Closed(object? sender, EventArgs e)
        {
            Close();
        }

        private void NumTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Any(x => !Char.IsDigit(x)))
                e.Handled = true;
        }
    }
}
