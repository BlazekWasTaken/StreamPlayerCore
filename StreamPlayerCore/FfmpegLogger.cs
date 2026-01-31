using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using FFmpeg.AutoGen;
using Microsoft.Extensions.Logging;

namespace StreamPlayerCore;

[SuppressMessage("Performance", "CA1873:Avoid potentially expensive logging")]
public class FfmpegLogger(ILogger<FfmpegLogger> logger)
{
    private Guid _instanceId;
    private av_log_set_callback_callback? _logCallback;

    public unsafe void Initialize(Guid? instanceId = null)
    {
        _instanceId = instanceId ?? Guid.NewGuid();
        
        _logCallback = (p0, level, format, vl) =>
        {
            if (level > ffmpeg.av_log_get_level()) return;

            const int lineSize = 1024;
            var lineBuffer = stackalloc byte[lineSize];
            var printPrefix = 1;
            ffmpeg.av_log_format_line(p0, level, format, vl, lineBuffer, lineSize, &printPrefix);
            var line = Marshal.PtrToStringAnsi((IntPtr)lineBuffer);
            if (line == null) return;

            Log(line.Trim(), level);
        };

        SetupLogging(ffmpegLogLevel); // TODO: IOptions
    }

    private void Log(string message, int level)
    {
        switch (level)
        {
            case ffmpeg.AV_LOG_PANIC:
            case ffmpeg.AV_LOG_FATAL:
                logger.LogCritical("Stream instance: {id}; FFmpeg {level}: {message}",
                    _instanceId,
                    LogLevelToString(level),
                    message);
                break;
            case ffmpeg.AV_LOG_ERROR:
                logger.LogError("Stream instance: {id}; FFmpeg {level}: {message}",
                    _instanceId,
                    LogLevelToString(level),
                    message);
                break;
            case ffmpeg.AV_LOG_WARNING:
                logger.LogWarning("Stream instance: {id}; FFmpeg {level}: {message}",
                    _instanceId,
                    LogLevelToString(level),
                    message);
                break;
            case ffmpeg.AV_LOG_INFO:
                logger.LogInformation("Stream instance: {id}; FFmpeg {level}: {message}",
                    _instanceId,
                    LogLevelToString(level),
                    message);
                break;
            case ffmpeg.AV_LOG_DEBUG:
                logger.LogDebug("Stream instance: {id}; FFmpeg {level}: {message}",
                    _instanceId,
                    LogLevelToString(level),
                    message);
                break;
            case ffmpeg.AV_LOG_VERBOSE:
            case ffmpeg.AV_LOG_TRACE:
                logger.LogTrace("Stream instance: {id}; FFmpeg {level}: {message}",
                    _instanceId,
                    LogLevelToString(level),
                    message);
                break;
        }
    }

    private void SetupLogging(int ffmpegLogLevel)
    {
        ffmpeg.av_log_set_level(ffmpegLogLevel);

        logger.LogInformation("Stream instance: {id}; FFmpeg logging set up with level: {logLevel} ({ffmpegLogLevel})",
            _instanceId,
            LogLevelToString(ffmpegLogLevel),
            ffmpegLogLevel);

        ffmpeg.av_log_set_callback(_logCallback);
    }

    private static string LogLevelToString(int ffmpegLogLevel)
    {
        return ffmpegLogLevel switch
        {
            ffmpeg.AV_LOG_PANIC => "Panic",
            ffmpeg.AV_LOG_FATAL => "Fatal",
            ffmpeg.AV_LOG_ERROR => "Error",
            ffmpeg.AV_LOG_WARNING => "Warning",
            ffmpeg.AV_LOG_INFO => "Info",
            ffmpeg.AV_LOG_DEBUG => "Debug",
            ffmpeg.AV_LOG_VERBOSE => "Verbose",
            ffmpeg.AV_LOG_TRACE => "Trace",
            ffmpeg.AV_LOG_QUIET => "Quiet",
            _ => "Unknown"
        };
    }
}