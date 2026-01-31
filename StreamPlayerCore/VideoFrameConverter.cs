using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;
using FFmpeg.AutoGen;
using Microsoft.Extensions.Logging;
using AVFrame = FFmpeg.AutoGen.AVFrame;
using AVPixelFormat = FFmpeg.AutoGen.AVPixelFormat;
using SwsContext = FFmpeg.AutoGen.SwsContext;
using SwsFlags = FFmpeg.AutoGen.SwsFlags;

namespace StreamPlayerCore;

[SuppressMessage("Performance", "CA1873:Avoid potentially expensive logging")]
public sealed unsafe class VideoFrameConverter(ILogger<VideoFrameConverter> logger) : IDisposable
{
    private bool _initialized;
    
    private Guid _instanceId;
    private Size _destinationSize;
    
    private IntPtr _convertedFrameBufferPtr;
    private byte_ptrArray4 _dstData;
    private int_array4 _dstLineSize;
    private SwsContext* _pConvertContext;

    public void Initialize(Size sourceSize, AVPixelFormat sourcePixelFormat, 
        Size destinationSize, AVPixelFormat destinationPixelFormat, 
        Guid instanceId = default)
    {
        _instanceId = instanceId;
        logger.LogInformation("Stream instance: {id}; Creating VideoFrameConverter.", _instanceId);
        
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
        
        _initialized = true;
    }

    public void Dispose()
    {
        if (!_initialized) return;
        
        logger.LogInformation("Stream instance: {id}; Disposing VideoFrameConverter.", _instanceId);

        Marshal.FreeHGlobal(_convertedFrameBufferPtr);
        ffmpeg.sws_freeContext(_pConvertContext);
    }

    public AVFrame Convert(AVFrame sourceFrame)
    {
        if (!_initialized)
            throw new InvalidOperationException("VideoFrameConverter is not initialized.");
        
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