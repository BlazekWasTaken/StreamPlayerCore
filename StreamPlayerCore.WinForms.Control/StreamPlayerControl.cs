using FFmpeg.AutoGen;
using Microsoft.Extensions.Logging;
using StreamPlayerCore.Helper;

namespace StreamPlayerCore.WinForms.Control;

public delegate void StreamStarted();

public delegate void StreamStopped(StreamStopReason reason);

public sealed partial class StreamPlayerControl : System.Windows.Forms.Control
{
    private readonly Lock _currentFrameLock = new();
    private readonly StreamPlayer _player;
    private readonly TimeSpan _timeout;
    private Bitmap? _currentFrame;

    public StreamPlayerControl(ILoggerFactory loggerFactory, TimeSpan timeout,
        RtspTransport transport = RtspTransport.Tcp, RtspFlags flags = RtspFlags.None,
        int analyzeDuration = 0, int probeSize = 65536,
        AVHWDeviceType hwDeviceType = AVHWDeviceType.AV_HWDEVICE_TYPE_NONE,
        FFmpegLogLevel ffmpegLogLevel = FFmpegLogLevel.AvLogQuiet)
    {
        InitializeComponent();
        DoubleBuffered = true;
        _timeout = timeout;
        Paint += (_, e) =>
        {
            lock (_currentFrameLock)
            {
                if (_currentFrame == null) return;
                e.Graphics.DrawImage(_currentFrame, 0, 0);
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

    public void StartStream(string url)
    {
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

    private void Player_FrameReadyEvent(Bitmap frame)
    {
        lock (_currentFrameLock)
        {
            _currentFrame?.Dispose();
            _currentFrame = new Bitmap(frame, new Size(Width, Height));
            frame.Dispose();
        }

        Invalidate();
    }
}