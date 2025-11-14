using System.Runtime.InteropServices;
using FFmpeg.AutoGen;
using SkiaSharp;

namespace StreamPlayerCore;

public enum RtspTransport
{
    Undefined = 0,
    Udp = 1,
    Tcp = 2,
    UdpMulticast = 3,
    Http = 4
}

public enum RtspFlags
{
    None = 0,
    FilterSrc = 1,
    Listen = 2,
    PreferTcp = 3
}

public delegate void FrameReady(SKBitmap frame);

public sealed class StreamPlayer
{
    private readonly AVHWDeviceType _hwDecodeDeviceType;
    private readonly unsafe AVDictionary* _optionsPtr;

    private readonly CancellationTokenSource _tokenSource = new();
    private bool _started;

    public StreamPlayer(RtspTransport transport = RtspTransport.Undefined, RtspFlags flags = RtspFlags.None,
        int analyzeDuration = 0, int probeSize = 65536,
        AVHWDeviceType hwDecodeDeviceType = AVHWDeviceType.AV_HWDEVICE_TYPE_NONE,
        int ffmpegLogLevel = ffmpeg.AV_LOG_VERBOSE)
    {
        ffmpeg.RootPath = Path.Join(Environment.CurrentDirectory, "ffmpeg", "bin");
        DynamicallyLoadedBindings.Initialize();
        Console.WriteLine($"FFmpeg version info: {ffmpeg.av_version_info()}");
        unsafe
        {
            _optionsPtr = GetAvDict(transport, flags, analyzeDuration, probeSize);
        }

        _hwDecodeDeviceType = hwDecodeDeviceType;
        SetupLogging(ffmpegLogLevel);
    }

    public event FrameReady? FrameReadyEvent;

    public unsafe void Start(Uri streamSource)
    {
        if (_started) return;
        _started = true;
        Task.Run(() =>
        {
            using var vsd = new VideoStreamDecoder(streamSource.AbsoluteUri, _hwDecodeDeviceType, _optionsPtr);

            if (vsd.FrameSize.Width == 0 || vsd.FrameSize.Height == 0 ||
                vsd.PixelFormat == AVPixelFormat.AV_PIX_FMT_NONE)
            {
                Console.WriteLine("Invalid video stream parameters. Stopping the stream.");
                Stop();
                return;
            }

            Console.WriteLine($"codec name: {vsd.CodecName}");
            Console.WriteLine($"source size: {vsd.FrameSize.Width}x{vsd.FrameSize.Height}");
            Console.WriteLine($"source pixel format: {vsd.PixelFormat}");

            var info = vsd.GetContextInfo();
            info.ToList().ForEach(x => Console.WriteLine($"{x.Key} = {x.Value}"));

            var sourceSize = vsd.FrameSize;
            var sourcePixelFormat = _hwDecodeDeviceType == AVHWDeviceType.AV_HWDEVICE_TYPE_NONE
                ? vsd.PixelFormat
                : GetHwPixelFormat(_hwDecodeDeviceType);
            // ReSharper disable once InlineTemporaryVariable
            var destinationSize = sourceSize;
            const AVPixelFormat destinationPixelFormat = AVPixelFormat.AV_PIX_FMT_BGRA;
            using var vfc =
                new VideoFrameConverter(sourceSize, sourcePixelFormat, destinationSize, destinationPixelFormat);

            while (!_tokenSource.IsCancellationRequested && vsd.TryDecodeNextFrame(out var frame))
            {
                var convertedFrame = vfc.Convert(frame);
                var imageInfo = new SKImageInfo(convertedFrame.width, convertedFrame.height, SKColorType.Bgra8888,
                    SKAlphaType.Opaque);
                var bitmap = new SKBitmap();

                bitmap.InstallPixels(imageInfo, (IntPtr)convertedFrame.data[0]);
                if (bitmap.IsEmpty || bitmap.IsNull ||
                    bitmap.Width == 0 || bitmap.Height == 0 ||
                    bitmap.BytesPerPixel == 0 || bitmap.RowBytes == 0 ||
                    bitmap.Info.ColorType == SKColorType.Unknown || bitmap.Info.AlphaType == SKAlphaType.Unknown ||
                    !bitmap.ReadyToDraw)
                {
                    Console.WriteLine("Invalid bitmap created from frame. Skipping frame.");
                    bitmap.Dispose();
                    continue;
                }

                Task.Delay(5).Wait();
                OnProcessCompleted(bitmap);
            }
        });
    }

    public void Stop()
    {
        if (!_started) return;
        _tokenSource.Cancel();
        _started = false;
    }

    private static unsafe void SetupLogging(int ffmpegLogLevel)
    {
        ffmpeg.av_log_set_level(ffmpegLogLevel);

        // ReSharper disable once ConvertToLocalFunction
        av_log_set_callback_callback logCallback = (p0, level, format, vl) =>
        {
            if (level > ffmpeg.av_log_get_level()) return;

            const int lineSize = 1024;
            var lineBuffer = stackalloc byte[lineSize];
            var printPrefix = 1;
            ffmpeg.av_log_format_line(p0, level, format, vl, lineBuffer, lineSize, &printPrefix);
            var line = Marshal.PtrToStringAnsi((IntPtr)lineBuffer);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(line);
            Console.ResetColor();
        };

        ffmpeg.av_log_set_callback(logCallback);
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

    private void OnProcessCompleted(SKBitmap frame)
    {
        FrameReadyEvent?.Invoke(frame);
    }
}