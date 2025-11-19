using System.Runtime.InteropServices;
using FFmpeg.AutoGen;

namespace StreamPlayerCore;

public class FFmpegInitException(string message) : Exception(message);

public class FFmpegException(int error) : Exception(AvStrError(error) ?? $"FFmpeg error code: {error}")
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

internal static class FFmpegExtensions
{
    public static bool RunWithTimeout(int timeoutMilliseconds, Action action)
    {
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(timeoutMilliseconds);
        var token = cts.Token;
        var task = Task.Run(action, token);
        try
        {
            task.Wait(token);
        }
        catch (OperationCanceledException e)
        {
            if (token.IsCancellationRequested)
                return false;
        }
        return true;
    }
}