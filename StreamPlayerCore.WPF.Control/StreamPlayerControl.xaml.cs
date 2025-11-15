using FFmpeg.AutoGen;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using StreamPlayerCore.Helper;

namespace StreamPlayerCore.WPF.Control;

public partial class StreamPlayerControl
{
    private readonly StreamPlayer _player;
    private SKBitmap? _currentFrame;
    private FitType _fitType;

    public StreamPlayerControl(ref ILoggerFactory loggerFactory, 
        RtspTransport transport = RtspTransport.Tcp, RtspFlags flags = RtspFlags.None,
        int analyzeDuration = 0, int probeSize = 65536,
        AVHWDeviceType hwDeviceType = AVHWDeviceType.AV_HWDEVICE_TYPE_NONE,
        FFmpegLogLevel ffmpegLogLevel = FFmpegLogLevel.AvLogDebug)
    {
        InitializeComponent();
        _player = new StreamPlayer(ref loggerFactory,
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
        Dispatcher.Invoke(SkiaControl.InvalidateVisual);
    }

    private void Player_FrameReadyEvent(SKBitmap frame)
    {
        _currentFrame?.Dispose();
        _currentFrame = frame;
        Dispatcher.Invoke(SkiaControl.InvalidateVisual);
    }

    private void SkiaControl_OnPaintSurface(object? _, SKPaintSurfaceEventArgs e)
    {
        SkiaHelper.SkControlOnPaintSurface(e, _currentFrame, _fitType);
    }
}