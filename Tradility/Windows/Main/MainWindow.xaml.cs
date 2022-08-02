using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tradility.Abstractions;
using Tradility.Abstractions.Services;
using Tradility.Common.Localization;
using Tradility.Windows.Settings;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Tradility.Windows.Main
{
    public partial class MainWindow : Window
    {
        // For Dragablz
        public static MainWindow? Instance { get; private set; }
        private static TabItem? tablesTabitem;
        private static TabItem? chartingTabitem;
        // For Dragablz
        public MainWindow()
        {
            InitializeComponent();

            DataContext = Instance?.DataContext;

            Instance = this;


            SetVersionValue();
        }

        private readonly IServiceProvider serviceProvider;

        public MainWindow(MainViewModel mainViewModel, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            var tablesContentControl = new ContentControl
            {
                Content = mainViewModel.TablesViewModel
            };
            tablesTabitem = new TabItem
            {
                Header = R.Tables,
                Content = tablesContentControl
            };

            var stocksContentControl = new ContentControl
            {
                Content = mainViewModel.StocksViewModel
            };
            chartingTabitem = new TabItem
            {
                Header = R.Charting,
                Content = stocksContentControl
            };

            DataContext = mainViewModel;
            this.serviceProvider = serviceProvider;

            tabControl.AddToSource(tablesTabitem);
            tabControl.AddToSource(chartingTabitem);

            Instance = this;
            SetVersionValue();
        }

        private void SetVersionValue()
        {
            var versionString =
#if DEBUG
                "DEBUG"
#else
                "-"
#endif
                ;

            try
            {
                Package package = Package.Current;
                PackageId packageId = package.Id;
                PackageVersion version = packageId.Version;

                versionString = string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
            }
            catch(Exception ex)
            {
                IOC.Instance.Resolve<ILoggerFactory>().CreateLogger(ex.ToString(), "Errors");
            }

            versionIdentifier.Content = $"{R.Version} {versionString}";
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ButtonState == MouseButtonState.Pressed)
                DragMove(); // TODO Gestures
        }

        private void MinimizeClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeClick(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            var tablesWindow = Window.GetWindow(tablesTabitem) as MainWindow;
            var stocksWindow = Window.GetWindow(chartingTabitem) as MainWindow;

            if (tablesWindow == null)
            {
                tablesWindow?.tabControl.AddToSource(chartingTabitem);
            }
            else if (stocksWindow == null)
            {
                stocksWindow?.tabControl.AddToSource(tablesTabitem);
            }
            else
            {
                if (tablesWindow == this)
                {
                    tablesWindow?.tabControl.Items.Clear();
                    stocksWindow?.tabControl.AddToSource(tablesTabitem);
                }
                else
                {
                    stocksWindow?.tabControl.Items.Clear();
                    tablesWindow?.tabControl.AddToSource(chartingTabitem);
                }
            }

            base.OnClosing(e);
        }

        private void Settings_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var settingsWindow = App.ServiceProvider!.GetService<SettingsWindow>();
            settingsWindow?.ShowDialog();
        }

        private void Manual_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var fileName = TLocalization.Instance.Language == Common.Localization.Language.English ? "Manual.en-US.pdf" : "Manual.de-DE.pdf";
            var tmpFolder = Path.GetTempPath();
            var filePath = Path.Combine(tmpFolder, fileName);

            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream($"Tradility.Properties.{fileName}")!;
            using var fileStream = File.Create(filePath);
            stream.CopyTo(fileStream);
            fileStream.Close();
            //Task.Delay(500).Wait();
            Process.Start(new ProcessStartInfo(filePath)
            {
                UseShellExecute = true,
            });
        }
    }
}
