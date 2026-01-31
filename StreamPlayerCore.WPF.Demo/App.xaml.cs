using System.Windows;
using Microsoft.Extensions.DependencyInjection;
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

        serviceCollection.AddStreamPlayerCoreServices(new FfmpegOptions
        {
            LogLevel = (int)FFmpegLogLevel.AvLogDebug,
            Timeout = TimeSpan.FromSeconds(10)
        });
        
        serviceCollection.AddSingleton<MainWindow>();
        
        var serviceProvider = serviceCollection.BuildServiceProvider();
        
        var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}