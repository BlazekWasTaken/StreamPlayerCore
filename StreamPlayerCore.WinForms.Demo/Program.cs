using FFmpeg.AutoGen;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using StreamPlayerCore.Enums;
using StreamPlayerCore.Options;
using StreamPlayerCore.WinForms.Control;

namespace StreamPlayerCore.WinForms.Demo;

internal static class Program
{
    /// <summary>
    ///     The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        
        var host = CreateHostBuilder().Build();
        var serviceProvider = host.Services;
        
        Application.Run(serviceProvider.GetRequiredService<DemoForm>());
    }

    private static IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) => {
                var serilogLogger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .WriteTo.File("log.txt", rollingInterval: RollingInterval.Minute)
                    .WriteTo.Console()
                    .CreateLogger();
                
                services.AddLogging(loggingBuilder =>
                {
                    loggingBuilder.ClearProviders();
                    loggingBuilder.AddSerilog(serilogLogger);
                });
                
                services.AddStreamPlayerCoreServices(new FfmpegOptions
                {
                    LogLevel = (int)FfmpegLogLevel.AvLogDebug,
                    HwDecodeDeviceType = AVHWDeviceType.AV_HWDEVICE_TYPE_NONE,
                    Timeout = TimeSpan.FromSeconds(10)
                });
                
                services.AddSingleton<DemoForm>();
            });
    }
}