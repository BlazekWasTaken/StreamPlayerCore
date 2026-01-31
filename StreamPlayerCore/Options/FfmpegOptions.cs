using FFmpeg.AutoGen;
using StreamPlayerCore.Helper;

namespace StreamPlayerCore.Options;

public class FfmpegOptions
{
    public int LogLevel { get; set; } = (int)FFmpegLogLevel.AvLogDebug;
    public AVHWDeviceType HwDecodeDeviceType { get; set; } = AVHWDeviceType.AV_HWDEVICE_TYPE_NONE;
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);
}