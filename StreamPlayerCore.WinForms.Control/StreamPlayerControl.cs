using Microsoft.Extensions.DependencyInjection;
using StreamPlayerCore.Enums;

namespace StreamPlayerCore.WinForms.Control;

public delegate void StreamStarted();

public delegate void StreamStopped(StreamStopReason reason);

public sealed partial class StreamPlayerControl : UserControl
{
    private readonly Lock _currentFrameLock = new();

    private readonly StreamPlayer _player;
    private Bitmap? _currentFrame;

    public StreamPlayerControl(IServiceScopeFactory serviceScopeFactory)
    {
        InitializeComponent();
        DoubleBuffered = true;

        Paint += (_, e) =>
        {
            lock (_currentFrameLock)
            {
                if (_currentFrame == null) return;
                e.Graphics.DrawImage(_currentFrame, 0, 0);
            }
        };

        using var scope = serviceScopeFactory.CreateScope();
        _player = scope.ServiceProvider.GetRequiredService<StreamPlayer>();

        _player.FrameReadyEvent += Player_FrameReadyEvent;
        _player.StreamStartedEvent += () => { StreamStartedEvent?.Invoke(); };
        _player.StreamStoppedEvent += reason => { StreamStoppedEvent?.Invoke(reason); };
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public Dictionary<string, string> Options { get; } = new();

    public event StreamStarted? StreamStartedEvent;
    public event StreamStopped? StreamStoppedEvent;

    public void StartStream(string url)
    {
        _player.Start(new Uri(url), Options);
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