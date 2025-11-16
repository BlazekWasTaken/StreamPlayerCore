using FFmpeg.AutoGen;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using StreamPlayerCore.Helper;

namespace StreamPlayerCore.WinForms.Control;

public partial class StreamPlayerControl : SKControl
{
    private readonly StreamPlayer _player;
    private SKBitmap? _currentFrame;
    private FitType _fitType;

    public StreamPlayerControl(ILoggerFactory loggerFactory, 
        RtspTransport transport = RtspTransport.Tcp, RtspFlags flags = RtspFlags.None,
        int analyzeDuration = 0, int probeSize = 65536,
        AVHWDeviceType hwDeviceType = AVHWDeviceType.AV_HWDEVICE_TYPE_NONE,
        FFmpegLogLevel ffmpegLogLevel = FFmpegLogLevel.AvLogQuiet)
    {
        InitializeComponent();
        PaintSurface += (_, e) => SkiaHelper.SkControlOnPaintSurface(e, _currentFrame, _fitType);
        _player = new StreamPlayer(loggerFactory,
            transport,
            flags,
            analyzeDuration,
            probeSize,
            hwDeviceType,
            (int)ffmpegLogLevel);
        _player.FrameReadyEvent += Player_FrameReadyEvent;
    }

    public void StartStream(string url, FitType fitType = FitType.Stretch)
    {
        _fitType = fitType;
        _player.Start(new Uri(url));
    }

    public void StopStream()
    {
        _player.Stop();
        _currentFrame?.Dispose();
        _currentFrame = null;
        Invalidate();
    }

    private void Player_FrameReadyEvent(SKBitmap frame)
    {
        _currentFrame?.Dispose();
        _currentFrame = frame;

        Invalidate();
    }
}