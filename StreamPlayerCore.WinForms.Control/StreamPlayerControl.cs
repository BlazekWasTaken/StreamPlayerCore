namespace StreamPlayerCore.WinForms.Control;

public enum FitType
{
    Stretch,
    Center
}

public sealed partial class StreamPlayerControl : UserControl
{
    private readonly StreamPlayer _player;
    private Bitmap? _currentFrame;
    private FitType _fitType;
    
    public StreamPlayerControl()
    {
        InitializeComponent();
        DoubleBuffered = true;
        _player = new StreamPlayer(@"C:\Users\blazej\Desktop\ffmpeg-8.0-full_build-shared\bin", RtspTransport.Tcp);
        _player.FrameReadyEvent += Player_FrameReadyEvent;
        Paint += StreamPlayerControl_Paint;
    }

    private void StreamPlayerControl_Paint(object? sender, PaintEventArgs e)
    {
        if (_currentFrame == null)
            return;
        e.Graphics.DrawImage(_currentFrame, 0, 0, Width, Height);
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
        Invoke(Invalidate);
    }

    private void Player_FrameReadyEvent(Bitmap frame)
    {
        _currentFrame?.Dispose();
        _currentFrame = frame;
        Invoke(Invalidate);
    }
}