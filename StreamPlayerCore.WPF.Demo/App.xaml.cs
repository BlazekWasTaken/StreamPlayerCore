using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Serilog;
using StreamPlayerCore.Helper;
using StreamPlayerCore.Options;
using StreamPlayerCore.WPF.Control;

namespace StreamPlayerCore.WPF.Demo;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private void App_OnStartup(object sender, StartupEventArgs e)
    {
        var serilogLogger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File("log.txt", rollingInterval: RollingInterval.Minute)
            .WriteTo.Console()
            .CreateLogger();
        
        var serviceCollection = new ServiceCollection();
        
        serviceCollection.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog(serilogLogger);
        });

        serviceCollection.Configure<FfmpegOptions>(options =>
        {
            options.LogLevel = (int)FFmpegLogLevel.AvLogQuiet;
        });
        
         serviceCollection.TryAddScoped<StreamPlayerControl>();
        serviceCollection.AddScoped<FfmpegLogger>();
        serviceCollection.AddScoped<StreamPlayer>();
        serviceCollection.AddScoped<StreamPlayerControl>();
        serviceCollection.AddSingleton<MainWindow>();
        
        var serviceProvider = serviceCollection.BuildServiceProvider();
        
        var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}