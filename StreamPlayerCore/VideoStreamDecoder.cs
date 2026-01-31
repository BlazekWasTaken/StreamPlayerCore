using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using FFmpeg.AutoGen;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StreamPlayerCore.Options;
using AVCodec = FFmpeg.AutoGen.AVCodec;
using AVCodecContext = FFmpeg.AutoGen.AVCodecContext;
using AVFormatContext = FFmpeg.AutoGen.AVFormatContext;
using AVFrame = FFmpeg.AutoGen.AVFrame;
using AVHWDeviceType = FFmpeg.AutoGen.AVHWDeviceType;
using AVMediaType = FFmpeg.AutoGen.AVMediaType;
using AVPacket = FFmpeg.AutoGen.AVPacket;
using AVPixelFormat = FFmpeg.AutoGen.AVPixelFormat;

namespace StreamPlayerCore;

[SuppressMessage("Performance", "CA1873:Avoid potentially expensive logging")]
public sealed unsafe class VideoStreamDecoder : IDisposable
{
    private bool _initialized;
    
    private Guid _instanceId;
    private readonly ILogger<VideoStreamDecoder> _logger;
    
    private readonly TimeSpan _timeout;
    
    public string CodecName { get; private set; } = string.Empty;
    public Size FrameSize { get; private set; }
    public AVPixelFormat PixelFormat { get; private set; }
    public AVHWDeviceType HwDecodeDeviceType { get; }
    
    private AVCodecContext* _pCodecContext;
    private AVFormatContext* _pFormatContext;
    private AVFrame* _pFrame;
    private AVPacket* _pPacket;
    private AVFrame* _receivedFrame;
    private int _streamIndex;
    
    public VideoStreamDecoder(ILogger<VideoStreamDecoder> logger, IOptions<FfmpegOptions> options)
    {
        _logger = logger;
        _timeout = options.Value.Timeout;
        HwDecodeDeviceType = options.Value.HwDecodeDeviceType;
    }

    public void Initialize(string url, AVDictionary* options = null, Guid? instanceId = null)
    {
        _instanceId = instanceId ?? Guid.NewGuid();
        _logger.LogInformation("Stream instance: {id}; Creating VideoStreamDecoder.", _instanceId);

        var success = FFmpegExtensions.RunWithTimeout(_timeout, () =>
        {
            var tempOptions = options;

            _pFormatContext = ffmpeg.avformat_alloc_context();
            _receivedFrame = ffmpeg.av_frame_alloc();
            var pFormatContext = _pFormatContext;
            ffmpeg.avformat_open_input(&pFormatContext, url, null, &tempOptions).ThrowExceptionIfError();
            ffmpeg.avformat_find_stream_info(_pFormatContext, null).ThrowExceptionIfError();
            AVCodec* codec = null;
            _streamIndex = ffmpeg
                .av_find_best_stream(_pFormatContext, AVMediaType.AVMEDIA_TYPE_VIDEO, -1, -1, &codec, 0)
                .ThrowExceptionIfError();
            _pCodecContext = ffmpeg.avcodec_alloc_context3(codec);

            if (HwDecodeDeviceType != AVHWDeviceType.AV_HWDEVICE_TYPE_NONE)
                ffmpeg.av_hwdevice_ctx_create(&_pCodecContext->hw_device_ctx, HwDecodeDeviceType, null, null, 0)
                    .ThrowExceptionIfError();

            ffmpeg.avcodec_parameters_to_context(_pCodecContext, _pFormatContext->streams[_streamIndex]->codecpar)
                .ThrowExceptionIfError();
            ffmpeg.avcodec_open2(_pCodecContext, codec, null).ThrowExceptionIfError();

            CodecName = ffmpeg.avcodec_get_name(codec->id);
            FrameSize = new Size(_pCodecContext->width, _pCodecContext->height);
            PixelFormat = _pCodecContext->pix_fmt;

            _pPacket = ffmpeg.av_packet_alloc();
            _pFrame = ffmpeg.av_frame_alloc();
        });
        if (!success)
        {
            _logger.LogError("Stream instance: {id}; Timeout while initializing VideoStreamDecoder.", _instanceId);
            throw new FFmpegInitException("Timeout while initializing VideoStreamDecoder.");
        }

        _logger.LogInformation("Stream instance: {id}; VideoStreamDecoder initialized successfully.", _instanceId);
        
        _initialized = true;
    }

    public void Dispose()
    {
        if (!_initialized)
            return;
        
        _logger.LogInformation("Stream instance: {id}; Disposing VideoStreamDecoder.", _instanceId);

        var pFrame = _pFrame;
        ffmpeg.av_frame_free(&pFrame);

        var pPacket = _pPacket;
        ffmpeg.av_packet_free(&pPacket);

        var avCodecContext = stackalloc[] { _pCodecContext };
        ffmpeg.avcodec_free_context(avCodecContext);

        var pFormatContext = _pFormatContext;
        ffmpeg.avformat_close_input(&pFormatContext);
    }

    public bool TryDecodeNextFrame(out AVFrame frame)
    {
        if (!_initialized)
            throw new InvalidOperationException("VideoStreamDecoder is not initialized.");
        
        ffmpeg.av_frame_unref(_pFrame);
        ffmpeg.av_frame_unref(_receivedFrame);
        int error;

        do
        {
            try
            {
                do
                {
                    ffmpeg.av_packet_unref(_pPacket);
                    error = ffmpeg.av_read_frame(_pFormatContext, _pPacket);

                    if (error == ffmpeg.AVERROR_EOF)
                    {
                        frame = *_pFrame;
                        return false;
                    }

                    error.ThrowExceptionIfError();
                } while (_pPacket->stream_index != _streamIndex);

                ffmpeg.avcodec_send_packet(_pCodecContext, _pPacket).ThrowExceptionIfError();
            }
            finally
            {
                ffmpeg.av_packet_unref(_pPacket);
            }

            error = ffmpeg.avcodec_receive_frame(_pCodecContext, _pFrame);
        } while (error == ffmpeg.AVERROR(ffmpeg.EAGAIN));

        error.ThrowExceptionIfError();

        if (_pCodecContext->hw_device_ctx != null)
        {
            ffmpeg.av_hwframe_transfer_data(_receivedFrame, _pFrame, 0).ThrowExceptionIfError();
            frame = *_receivedFrame;
        }
        else
        {
            frame = *_pFrame;
        }

        return true;
    }
}