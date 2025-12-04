using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;
using FFmpeg.AutoGen;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using AVFrame = FFmpeg.AutoGen.AVFrame;
using AVPixelFormat = FFmpeg.AutoGen.AVPixelFormat;
using SwsContext = FFmpeg.AutoGen.SwsContext;
using SwsFlags = FFmpeg.AutoGen.SwsFlags;

namespace StreamPlayerCore;

[SuppressMessage("Performance", "CA1873:Avoid potentially expensive logging")]
public sealed unsafe class VideoFrameConverter : IDisposable
{
    private readonly IntPtr _convertedFrameBufferPtr;
    private readonly Size _destinationSize;
    private readonly byte_ptrArray4 _dstData;
    private readonly int_array4 _dstLineSize;
    private readonly Guid _instanceId;

    private readonly ILogger<VideoFrameConverter> _logger;
    private readonly SwsContext* _pConvertContext;

    public VideoFrameConverter(ILoggerFactory loggerFactory,
        Size sourceSize, AVPixelFormat sourcePixelFormat,
        Size destinationSize, AVPixelFormat destinationPixelFormat,
        Guid instanceId = default)
    {
        _logger = loggerFactory?.CreateLogger<VideoFrameConverter>()
                  ?? NullLoggerFactory.Instance.CreateLogger<VideoFrameConverter>();
        _instanceId = instanceId;

        _logger.LogInformation("Stream instance: {id}; Creating VideoFrameConverter.", _instanceId);

        _destinationSize = destinationSize;

        _pConvertContext = ffmpeg.sws_getContext(sourceSize.Width,
            sourceSize.Height,
            sourcePixelFormat,
            destinationSize.Width,
            destinationSize.Height,
            destinationPixelFormat,
            (int)SwsFlags.SWS_SINC,
            null,
            null,
            null);
        if (_pConvertContext == null)
            throw new ApplicationException("Could not initialize the conversion context.");

        var convertedFrameBufferSize = ffmpeg.av_image_get_buffer_size(destinationPixelFormat,
            destinationSize.Width,
            destinationSize.Height,
            1);
        _convertedFrameBufferPtr = Marshal.AllocHGlobal(convertedFrameBufferSize);
        _dstData = new byte_ptrArray4();
        _dstLineSize = new int_array4();

        ffmpeg.av_image_fill_arrays(ref _dstData,
            ref _dstLineSize,
            (byte*)_convertedFrameBufferPtr,
            destinationPixelFormat,
            destinationSize.Width,
            destinationSize.Height,
            1);
    }

    public void Dispose()
    {
        _logger.LogInformation("Stream instance: {id}; Disposing VideoFrameConverter.", _instanceId);

        Marshal.FreeHGlobal(_convertedFrameBufferPtr);
        ffmpeg.sws_freeContext(_pConvertContext);
    }

    public AVFrame Convert(AVFrame sourceFrame)
    {
        ffmpeg.sws_scale(_pConvertContext,
            sourceFrame.data,
            sourceFrame.linesize,
            0,
            sourceFrame.height,
            _dstData,
            _dstLineSize);

        var data = new byte_ptrArray8();
        data.UpdateFrom(_dstData);
        var lineSize = new int_array8();
        lineSize.UpdateFrom(_dstLineSize);

        return new AVFrame
        {
            data = data,
            linesize = lineSize,
            width = _destinationSize.Width,
            height = _destinationSize.Height
        };
    }
}