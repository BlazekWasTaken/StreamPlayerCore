using System.Windows;
using Microsoft.Extensions.Logging;

namespace StreamPlayerCore.WPF.Demo;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private void App_OnStartup(object sender, StartupEventArgs e)
    { 
        var loggerFactory = LoggerFactory.Create(static builder =>
        {
            builder
                .AddFilter("Microsoft", LogLevel.Warning)
                .AddFilter("System", LogLevel.Warning)
                .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
                .AddConsole();
        });
        
        var mainWindow = new MainWindow(loggerFactory);
        mainWindow.Show();
    }
}