using Microsoft.Extensions.DependencyInjection;
using StreamPlayerCore.Enums;

namespace StreamPlayerCore.WinForms.Control;

public delegate void StreamStarted();

public delegate void StreamStopped(StreamStopReason reason);

public class PlayerOptions
{
    public RtspTransport Transport { get; set; } = RtspTransport.Tcp;
    public RtspFlags Flags { get; set; } = RtspFlags.None;
    public int AnalyzeDuration { get; set; } = 0;
    public int ProbeSize { get; set; } = 65536;
}

public sealed partial class StreamPlayerControl : System.Windows.Forms.Control
{
    // ReSharper disable once MemberCanBePrivate.Global
    public PlayerOptions Options { get; } = new();
    
    private readonly StreamPlayer _player;
    
    private readonly Lock _currentFrameLock = new();
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

    public event StreamStarted? StreamStartedEvent;
    public event StreamStopped? StreamStoppedEvent;

    public void StartStream(string url)
    {
        _player.Start(new Uri(url), 
            Options.Transport,
            Options.Flags,
            Options.AnalyzeDuration,
            Options.ProbeSize);
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