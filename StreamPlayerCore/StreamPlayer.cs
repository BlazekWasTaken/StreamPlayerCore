using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using FFmpeg.AutoGen;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StreamPlayerCore.Enums;

namespace StreamPlayerCore;

public delegate void FrameReady(Bitmap frame);

public delegate void StreamStarted();

public delegate void StreamStopped(StreamStopReason reason);

[SuppressMessage("Performance", "CA1873:Avoid potentially expensive logging")]
public sealed class StreamPlayer
{
    private readonly Guid _instanceId = Guid.NewGuid();
    private readonly ILogger<StreamPlayer> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    
    private bool _started;

    private CancellationTokenSource _tokenSource = new();

    public StreamPlayer(ILogger<StreamPlayer> logger, FfmpegLogger ffmpegLogger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;

        ffmpeg.RootPath = Path.Join(Environment.CurrentDirectory, "ffmpeg");
        DynamicallyLoadedBindings.Initialize();
        _logger.LogInformation(
            "Stream instance: {id}; FFmpeg libraries loaded from: {ffmpegRootPath}; Version info: {ffmpegVersionInfo}",
            _instanceId,
            ffmpeg.RootPath,
            ffmpeg.av_version_info());
        
        ffmpegLogger.Initialize(_instanceId);
    }

    public event FrameReady? FrameReadyEvent;
    public event StreamStarted? StreamStartedEvent;
    public event StreamStopped? StreamStoppedEvent;

    public unsafe void Start(Uri streamSource, RtspTransport transport = RtspTransport.Undefined, 
        RtspFlags flags = RtspFlags.None, int analyzeDuration = 0, int probeSize = 65536)
    {
        if (_started) return;
        _started = true;
        _tokenSource = new CancellationTokenSource();

        var sourceWithoutCredentials = new UriBuilder(streamSource)
        {
            UserName = string.Empty,
            Password = string.Empty
        }.Uri;
        _logger.LogInformation("Stream instance: {id}; Starting stream from source: {source}",
            _instanceId,
            sourceWithoutCredentials.ToString());

        Task.Run(() =>
        {
            var scope = _serviceScopeFactory.CreateScope();
            VideoStreamDecoder? vsd = null;
            var optionsPtr = GetAvDict(transport, flags, analyzeDuration, probeSize);
            try
            {
                vsd = scope.ServiceProvider.GetRequiredService<VideoStreamDecoder>();
                vsd.Initialize(streamSource.AbsoluteUri, optionsPtr, _instanceId);
            }
            catch (FFmpegInitException)
            {
                _logger.LogInformation(
                    "Stream instance: {id}; Failed to initialize video stream decoder.",
                    _instanceId);
                vsd?.Dispose();
                vsd = null;
                Stop(StreamStopReason.InitializationFailed);
                return;
            }

            if (vsd.FrameSize.Width == 0 || vsd.FrameSize.Height == 0 ||
                vsd.PixelFormat == AVPixelFormat.AV_PIX_FMT_NONE)
            {
                _logger.LogError("Stream instance: {id}; Invalid video stream parameters.",
                    _instanceId);
                vsd?.Dispose();
                vsd = null;
                Stop(StreamStopReason.InitializationFailed);
                return;
            }

            _logger.LogInformation("Stream instance: {id}; Video stream opened; Codec: {codecName}; " +
                                   "Source size: {width}x{height}; Source pixel format: {pixelFormat}",
                _instanceId,
                vsd.CodecName,
                vsd.FrameSize.Width,
                vsd.FrameSize.Height,
                vsd.PixelFormat);

            var sourceSize = vsd.FrameSize;
            var sourcePixelFormat = vsd.HwDecodeDeviceType == AVHWDeviceType.AV_HWDEVICE_TYPE_NONE
                ? vsd.PixelFormat
                : GetHwPixelFormat(vsd.HwDecodeDeviceType);
            // ReSharper disable once InlineTemporaryVariable
            var destinationSize = sourceSize;
            const AVPixelFormat destinationPixelFormat = AVPixelFormat.AV_PIX_FMT_BGRA;
            
            using var vfc = scope.ServiceProvider.GetRequiredService<VideoFrameConverter>();
            vfc.Initialize(sourceSize,
                sourcePixelFormat,
                destinationSize,
                destinationPixelFormat,
                _instanceId);

            var started = false;
            try
            {
                while (!_tokenSource.IsCancellationRequested && vsd.TryDecodeNextFrame(out var frame))
                {
                    if (!started)
                    {
                        OnStreamStarted();
                        started = true;
                    }

                    var convertedFrame = vfc.Convert(frame);
#pragma warning disable CA1416
                    var bitmap = new Bitmap(convertedFrame.width, convertedFrame.height, convertedFrame.linesize[0],
                        PixelFormat.Format32bppRgb, (IntPtr)convertedFrame.data[0]);
#pragma warning restore CA1416
                    OnFrameReady(bitmap);
                }
            }
            catch (FFmpegException e)
            {
                _logger.LogError("Stream instance: {id}; FFmpeg exception occurred: {message}",
                    _instanceId,
                    e.Message);
            }

            vsd.Dispose();
            vsd = null;
            if (!_tokenSource.IsCancellationRequested) Stop(StreamStopReason.StreamEnded);
        }, _tokenSource.Token);
    }

    public void Stop(StreamStopReason reason = StreamStopReason.UserRequested)
    {
        if (!_started) return;
        _started = false;

        _logger.LogInformation("Stream instance: {id}; Stopping stream.", _instanceId);

        _tokenSource.Cancel();
        OnStreamStopped(reason);
    }

    private static unsafe AVDictionary* GetAvDict(RtspTransport transport, RtspFlags flags, int analyzeDuration,
        int probeSize)
    {
        AVDictionary* optionsPtr = null;
        switch (transport)
        {
            case RtspTransport.Http:
                ffmpeg.av_dict_set(&optionsPtr, "rtsp_transport", "http", 0);
                break;
            case RtspTransport.Udp:
                ffmpeg.av_dict_set(&optionsPtr, "rtsp_transport", "udp", 0);
                break;
            case RtspTransport.Tcp:
                ffmpeg.av_dict_set(&optionsPtr, "rtsp_transport", "tcp", 0);
                break;
            case RtspTransport.UdpMulticast:
                ffmpeg.av_dict_set(&optionsPtr, "rtsp_transport", "udp_multicast", 0);
                break;
            case RtspTransport.Undefined:
            default:
                break;
        }

        switch (flags)
        {
            case RtspFlags.FilterSrc:
                ffmpeg.av_dict_set(&optionsPtr, "rtsp_flags", "filter_src", 0);
                break;
            case RtspFlags.Listen:
                ffmpeg.av_dict_set(&optionsPtr, "rtsp_flags", "listen", 0);
                break;
            case RtspFlags.PreferTcp:
                ffmpeg.av_dict_set(&optionsPtr, "rtsp_flags", "prefer_tcp", 0);
                break;
            case RtspFlags.None:
            default:
                break;
        }

        ffmpeg.av_dict_set(&optionsPtr, "analyzeduration", analyzeDuration.ToString(), 0);
        ffmpeg.av_dict_set(&optionsPtr, "probesize", probeSize.ToString(), 0);

        return optionsPtr;
    }

    private static AVPixelFormat GetHwPixelFormat(AVHWDeviceType hWDevice)
    {
        return hWDevice switch
        {
            AVHWDeviceType.AV_HWDEVICE_TYPE_NONE => AVPixelFormat.AV_PIX_FMT_NONE,
            AVHWDeviceType.AV_HWDEVICE_TYPE_VDPAU => AVPixelFormat.AV_PIX_FMT_VDPAU,
            AVHWDeviceType.AV_HWDEVICE_TYPE_CUDA => AVPixelFormat.AV_PIX_FMT_CUDA,
            AVHWDeviceType.AV_HWDEVICE_TYPE_VAAPI => AVPixelFormat.AV_PIX_FMT_VAAPI,
            AVHWDeviceType.AV_HWDEVICE_TYPE_DXVA2 => AVPixelFormat.AV_PIX_FMT_NV12,
            AVHWDeviceType.AV_HWDEVICE_TYPE_QSV => AVPixelFormat.AV_PIX_FMT_QSV,
            AVHWDeviceType.AV_HWDEVICE_TYPE_VIDEOTOOLBOX => AVPixelFormat.AV_PIX_FMT_VIDEOTOOLBOX,
            AVHWDeviceType.AV_HWDEVICE_TYPE_D3D11VA => AVPixelFormat.AV_PIX_FMT_NV12,
            AVHWDeviceType.AV_HWDEVICE_TYPE_DRM => AVPixelFormat.AV_PIX_FMT_DRM_PRIME,
            AVHWDeviceType.AV_HWDEVICE_TYPE_OPENCL => AVPixelFormat.AV_PIX_FMT_OPENCL,
            AVHWDeviceType.AV_HWDEVICE_TYPE_MEDIACODEC => AVPixelFormat.AV_PIX_FMT_MEDIACODEC,
            _ => AVPixelFormat.AV_PIX_FMT_NONE
        };
    }

    private void OnFrameReady(Bitmap frame)
    {
        FrameReadyEvent?.Invoke(frame);
    }

    private void OnStreamStarted()
    {
        _logger.LogInformation("Stream instance: {id}; Stream started.", _instanceId);
        StreamStartedEvent?.Invoke();
    }

    private void OnStreamStopped(StreamStopReason reason)
    {
        _logger.LogInformation("Stream instance: {id}; Stream stopped; Reason: {reason}", _instanceId, reason);
        StreamStoppedEvent?.Invoke(reason);
    }
}