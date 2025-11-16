using System.Runtime.InteropServices;
using FFmpeg.AutoGen;

namespace StreamPlayerCore;

public class FFmpegException(int error) : ApplicationException(AvStrError(error) ?? $"FFmpeg error code: {error}")
{
    private static unsafe string? AvStrError(int error)
    {
        const int bufferSize = 1024;
        var buffer = stackalloc byte[bufferSize];
        ffmpeg.av_strerror(error, buffer, bufferSize);
        var message = Marshal.PtrToStringAnsi((IntPtr)buffer);
        return message;
    }
}

internal static class FFmpegHelper
{
    public static int ThrowExceptionIfError(this int error)
    {
        return error < 0 ? throw new FFmpegException(error) : error;
    }
}