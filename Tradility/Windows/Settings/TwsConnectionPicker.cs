using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Tradility.ViewModels;

namespace Tradility.Windows.Settings
{
    public class TwsConnectionPicker : ComboBox
    {
        public TwsConnectionPicker()
        {
            Style = (Style)Application.Current.Resources[typeof(ComboBox)];
            ItemsSource = TwsConfigurationViewModel.Instance.Connections;
            SelectedItem = TwsConfigurationViewModel.Instance.ActiveConnection;
            TwsConfigurationViewModel.Instance.PropertyChanged += ConfigVMPropertyChanged;
            TwsConfigurationViewModel.Instance.Connections.CollectionChanged += Connections_CollectionChanged;
            SelectionChanged += TwsConnectionPicker_SelectionChanged;
        }

        private void Connections_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ItemsSource = TwsConfigurationViewModel.Instance.Connections;
            SelectedItem = TwsConfigurationViewModel.Instance.ActiveConnection;
        }

        private void ConfigVMPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TwsConfigurationViewModel.ActiveConnection))
            {
                SelectedItem = TwsConfigurationViewModel.Instance.ActiveConnection;
            }
        }

        private void TwsConnectionPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count > 0 && e.AddedItems[0] is TwsConnectionInfo tci)
            {
                TwsConfigurationViewModel.Instance.PropertyChanged -= ConfigVMPropertyChanged;
                TwsConfigurationViewModel.Instance.ActiveConnection = tci;
                TwsConfigurationViewModel.Instance.PropertyChanged += ConfigVMPropertyChanged;
            }
        }
    }
}
