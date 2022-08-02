global using R = Tradility.Properties.Resources;
global using Tradility.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using Tradility.Common.Extensions;
using Tradility.Common.Localization;
using Tradility.Common.Services;
using Tradility.Data.Repositories;
using Tradility.Data.Services;
using Tradility.Forms.CashReports;
using Tradility.Forms.Positions;
using Tradility.Forms.SideBar;
using Tradility.Forms.Stocks;
using Tradility.Forms.Tables;
using Tradility.Forms.Trades;
using Tradility.Properties;
using Tradility.Windows.Main;
using System.Linq;
using Tradility.Windows.Settings;
using System.IO;
using Tradility.Forms.Dividends;
using Tradility.ViewModels;
using Tradility.Abstractions;
using Tradility.Abstractions.Extensions;
using Tradility.Abstractions.Services;

namespace Tradility
{
    public partial class App : Application
    {
        public static ServiceProvider? ServiceProvider { get; private set; }

        private WebApplication? stocksWebApp;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ConfigureAppState();

            LoadSettings();

            var stocksWebAppBuilder = StocksWeb.App.CreateBuilder();
            ConfigureServices(stocksWebAppBuilder.Services);
            ServiceProvider = stocksWebAppBuilder.Services.BuildServiceProvider();

            stocksWebApp = StocksWeb.App.Build(stocksWebAppBuilder);
            var serviceProvider = stocksWebApp.Services;
            var mainWindow = serviceProvider.GetService<MainWindow>();

            stocksWebApp.RunAsync("http://localhost:9999");         
            mainWindow?.Show();
            IOC.Instance.Resolve<ILoggerFactory>().CreateLogger("App started", "AppExecution").Log();
        }

        private void ConfigureAppState()
        {
            var settings = Settings.Default;

            {//logger
                var path = settings.LoggingPath;
                if (string.IsNullOrWhiteSpace(Settings.Default.LoggingPath))
                {
                    path = Path.Combine(Path.GetTempPath(), "Tradility", "Logs");
                    Directory.CreateDirectory(path);
                    settings.LoggingPath = path;
                }

                IOC.Instance.AddLogger(settings.EnableLogging, path);
            }
            settings.Save();
        }
        private void ConfigureServices(IServiceCollection services)
        {
            // Windows
            services.AddSingleton<MainWindow>()
                    .AddTransient<SettingsWindow>();

            services.AddSingleton<TWS.Client>();

            // Services
            services.AddSingleton<CacheService>()
                    .AddSingleton<ExchangeService>()
                    .AddSingleton<DumpService>();

            // Repositories
            services.AddSingleton<TradeRepository>()
                    .AddSingleton<CashReportRepository>()
                    .AddSingleton<ExchangeRateRepository>()
                    .AddSingleton<FinancialInstrumentInfoRepository>()
                    .AddSingleton<PositionRepository>()
                    .AddSingleton<BarRepository>()
                    .AddSingleton<DividendRepository>();

            // ViewModels
            services.AddSingleton<MainViewModel>()
                    .AddTransient<SettingsViewModel>()
                    .AddSingleton<TablesViewModel>()
                    .AddSingleton<TradesViewModel>()
                    .AddSingleton<TradesMiniViewModel>()
                    .AddSingleton<CashReportsViewModel>()
                    .AddSingleton<PositionsViewModel>()
                    .AddSingleton<PositionsMiniViewModel>()
                    .AddSingleton<StocksViewModel>()                    
                    .AddTransient<SideBarViewModel>()
                    .AddSingleton<DividendsViewModel>()
                    .AddSingleton<DividendsMiniViewModel>();
        }

        private static void LoadSettings()
        {
            Res.Culture = Settings.Default.Language.ToCultureInfo();
            TLocalization.Instance.Language = Settings.Default.Language.ToLanguage();
            TwsConfigurationViewModel.Instance.Load();
        }

        protected async override void OnExit(ExitEventArgs e)
        {
            IOC.Instance.Resolve<ILoggerFactory>().CreateLogger("App exited", "AppExecution").Log();
            base.OnExit(e);

            if(stocksWebApp != null)
            {
                await stocksWebApp.StopAsync();
                await stocksWebApp.DisposeAsync();
            }            
        }
    }
}
