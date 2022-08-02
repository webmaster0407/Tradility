using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.Reflection;
using Tradility.TWS;
using TradingViewUdfProvider;

namespace Tradility.StocksWeb;

public static class App
{
    public static WebApplicationBuilder CreateBuilder()
    {        
        var builder = WebApplication.CreateBuilder();
        var services = builder.Services;

        var assembly = Assembly.GetExecutingAssembly();
        services
            .AddControllers()
            .AddApplicationPart(assembly)
            .AddJsonOptions(opts =>
            {
                opts.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            });

        services
            .AddDirectoryBrowser();
        services
            .AddMemoryCache()
            .AddSingleton<Client>()
            .AddSingleton<UDFProvider>();

        builder.Services.AddDirectoryBrowser();
        return builder;
    }

    public static WebApplication Build(WebApplicationBuilder builder)
    {
        var app = builder.Build();

        var assembly = typeof(App).GetTypeInfo().Assembly;
        var embeddedFileProvider = new EmbeddedFileProvider(
            assembly,
            "Tradility.StocksWeb.wwwroot"
        );

        app.UseDefaultFiles(new DefaultFilesOptions
        {
            FileProvider = embeddedFileProvider
        });
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = embeddedFileProvider,
        });
        app.UseDirectoryBrowser(new DirectoryBrowserOptions
        {
            FileProvider = embeddedFileProvider,
            RequestPath = "/statics"
        });
        
        app.MapControllers();

        return app;
    }
}


