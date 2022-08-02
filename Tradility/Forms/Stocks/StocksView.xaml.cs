using Microsoft.Web.WebView2.Core;
using PubSub;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Tradility.Messages;

namespace Tradility.Forms.Stocks
{
    public partial class StocksView : UserControl
    {
        private bool webViewInitialized;

        public StocksView()
        {
            var webview2CacheDirectory = "Tradility.exe.WebView2"; // TODO Settings
            if (Directory.Exists(webview2CacheDirectory))
                Directory.Delete(webview2CacheDirectory, true); 

            InitializeComponent();

            webView.CoreWebView2InitializationCompleted += WebView_CoreWebView2InitializationCompleted;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if(e.Property.Name == nameof(DataContext) && e.NewValue is StocksViewModel vm)
                vm.ScriptExecuted += ScriptExecuted; // TODO Interface for VM
        }

        private async void ScriptExecuted(object? sender, Common.Utils.EventArgs<string> e)
        {
            if (!string.IsNullOrWhiteSpace(e.Value) && webViewInitialized)
                await webView.CoreWebView2.ExecuteScriptAsync(e.Value);
        }

        private async void WebView_CoreWebView2InitializationCompleted(object? sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                await webView.EnsureCoreWebView2Async();
                webViewInitialized = true;

                await Hub.Default.PublishAsync(new StocksInitializedMessage());
            }
        }
    }
}
