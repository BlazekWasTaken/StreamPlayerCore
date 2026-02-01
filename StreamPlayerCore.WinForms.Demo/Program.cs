using FFmpeg.AutoGen;
using Microsoft.Extensions.DependencyInjection;
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
        ApplicationConfiguration.Initialize();
        
        var serviceCollection = new ServiceCollection();
        
        var serilogLogger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File("log.txt", rollingInterval: RollingInterval.Minute)
            .WriteTo.Console()
            .CreateLogger();
        
        serviceCollection.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog(serilogLogger);
        });
        serviceCollection.AddStreamPlayerCoreServices(new FfmpegOptions
        {
            LogLevel = (int)FfmpegLogLevel.AvLogDebug,
            HwDecodeDeviceType = AVHWDeviceType.AV_HWDEVICE_TYPE_NONE,
            Timeout = TimeSpan.FromSeconds(10)
        });
        serviceCollection.AddSingleton<DemoForm>();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        
        Application.Run(serviceProvider.GetRequiredService<DemoForm>());
    }
}