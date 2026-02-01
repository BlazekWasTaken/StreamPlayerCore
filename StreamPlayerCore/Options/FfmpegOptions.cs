using FFmpeg.AutoGen;
using StreamPlayerCore.Enums;

namespace StreamPlayerCore.Options;

public class FfmpegOptions
{
    public int LogLevel { get; set; } = (int)FfmpegLogLevel.AvLogDebug;
    public AVHWDeviceType HwDecodeDeviceType { get; set; } = AVHWDeviceType.AV_HWDEVICE_TYPE_NONE;
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);
}