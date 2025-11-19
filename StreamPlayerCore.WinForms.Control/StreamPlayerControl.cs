using FFmpeg.AutoGen;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using StreamPlayerCore.Helper;

namespace StreamPlayerCore.WinForms.Control;

public delegate void StreamStarted();

public delegate void StreamStopped(StreamStopReason reason);

public partial class StreamPlayerControl : SKControl
{
    private readonly Lock _currentFrameLock = new();
    private readonly StreamPlayer _player;
    private readonly TimeSpan _timeout;
    private SKBitmap? _currentFrame;
    private FitType _fitType;

    public StreamPlayerControl(ILoggerFactory loggerFactory, TimeSpan timeout,
        RtspTransport transport = RtspTransport.Tcp, RtspFlags flags = RtspFlags.None,
        int analyzeDuration = 0, int probeSize = 65536,
        AVHWDeviceType hwDeviceType = AVHWDeviceType.AV_HWDEVICE_TYPE_NONE,
        FFmpegLogLevel ffmpegLogLevel = FFmpegLogLevel.AvLogQuiet)
    {
        InitializeComponent();
        _timeout = timeout;
        PaintSurface += (_, e) =>
        {
            lock (_currentFrameLock)
            {
                SkiaHelper.SkControlOnPaintSurface(e, _currentFrame, _fitType);
            }
        };
        _player = new StreamPlayer(loggerFactory,
            transport,
            flags,
            analyzeDuration,
            probeSize,
            hwDeviceType,
            (int)ffmpegLogLevel);
        _player.FrameReadyEvent += Player_FrameReadyEvent;
        _player.StreamStartedEvent += () => { StreamStartedEvent?.Invoke(); };
        _player.StreamStoppedEvent += reason => { StreamStoppedEvent?.Invoke(reason); };
    }

    public event StreamStarted? StreamStartedEvent;
    public event StreamStopped? StreamStoppedEvent;

    public void StartStream(string url, FitType fitType = FitType.Stretch)
    {
        _fitType = fitType;
        _player.Start(new Uri(url), _timeout);
    }

    public void StopStream()
    {
        _player.Stop();
        lock (_currentFrameLock)
        {
            _currentFrame?.Dispose();
            _currentFrame = null;
        }

        Invalidate();
    }

    private void Player_FrameReadyEvent(SKBitmap frame)
    {
        lock (_currentFrameLock)
        {
            _currentFrame?.Dispose();
            _currentFrame = frame;
        }

        Invalidate();
    }
}