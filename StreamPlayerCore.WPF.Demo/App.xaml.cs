using System.Windows;
using Microsoft.Extensions.Logging;
using Serilog;

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
        
        var loggerFactory = new LoggerFactory().AddSerilog(serilogLogger);
        
        var mainWindow = new MainWindow(loggerFactory);
        mainWindow.Show();
    }
}