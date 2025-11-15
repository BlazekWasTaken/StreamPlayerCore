using Microsoft.Extensions.Logging;
using Serilog;

namespace StreamPlayerCore.WinForms.Demo;

internal static class Program
{
    /// <summary>
    ///     The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        
        var serilogLogger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File("log.txt", rollingInterval: RollingInterval.Minute)
            .WriteTo.Console()
            .CreateLogger();
        
        var loggerFactory = new LoggerFactory().AddSerilog(serilogLogger);
        
        Application.Run(new DemoForm(loggerFactory));
    }
}