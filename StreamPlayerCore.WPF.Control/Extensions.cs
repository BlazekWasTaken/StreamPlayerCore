using Microsoft.Extensions.DependencyInjection;
using StreamPlayerCore.Options;

namespace StreamPlayerCore.WPF.Control;

public static class Extensions
{
    public static void AddStreamPlayerCoreServices(this IServiceCollection services, FfmpegOptions? ffmpegOptions = null)
    {
        ffmpegOptions ??= new FfmpegOptions();
        
        services.Configure<FfmpegOptions>(options =>
        {
            options.LogLevel = ffmpegOptions.LogLevel;
            options.HwDecodeDeviceType = ffmpegOptions.HwDecodeDeviceType;
            options.Timeout = ffmpegOptions.Timeout;
        });
        
        services.AddScoped<VideoStreamDecoder>();
        services.AddScoped<VideoFrameConverter>();
        services.AddSingleton<FfmpegLogger>();
        services.AddScoped<StreamPlayer>();
        services.AddScoped<StreamPlayerControl>();
    }
}