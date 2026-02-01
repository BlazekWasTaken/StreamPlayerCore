namespace StreamPlayerCore.Enums;

public enum FfmpegLogLevel
{
    AvLogDebug = 0x30,
    AvLogError = 0x10,
    AvLogFatal = 0x8,
    AvLogInfo = 0x20,
    AvLogPanic = 0x0,
    AvLogQuiet = -0x8,
    AvLogTrace = 0x38,
    AvLogVerbose = 0x28,
    AvLogWarning = 0x18
}