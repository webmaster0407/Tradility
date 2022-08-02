using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Tradility.ViewModels;

namespace Tradility.Windows.Settings
{
    /// <summary>
    /// Interaction logic for TwsConnectionWindow.xaml
    /// </summary>
    public partial class TwsConnectionWindow : Window
    {
        public TwsConnectionWindow(TwsConnectionInfo? twsConnectionInfo)
        {
            InitializeComponent();

            if (twsConnectionInfo is not null)
            {
                displayName.Text = twsConnectionInfo.DisplayName;
                host.Text = twsConnectionInfo.Host;
                port.Text = twsConnectionInfo.Port.ToString();
                clientId.Text = twsConnectionInfo.ClientId.ToString();
            }
        }

        public TwsConnectionInfo? Result { get; private set; }

        private static readonly Regex _regex = new("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void PreviewTextInptInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void TextIntInputPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(displayName.Text) || string.IsNullOrWhiteSpace(host.Text) || !int.TryParse(port.Text, out var portValue) || !int.TryParse(clientId.Text, out var clientIdValue))
            {
                MessageBox.Show("Check input values!");
                return; ;
            }
            Result = new TwsConnectionInfo(displayName.Text, host.Text, portValue, clientIdValue);
            this.DialogResult = true;
        }
    }
}
