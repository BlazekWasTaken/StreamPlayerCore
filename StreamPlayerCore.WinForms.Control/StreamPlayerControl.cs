using SkiaSharp;
using SkiaSharp.Views.Desktop;
using StreamPlayerCore.Helper;

namespace StreamPlayerCore.WinForms.Control;

public partial class StreamPlayerControl : SKControl
{
    private readonly StreamPlayer _player;
    private SKBitmap? _currentFrame;
    private FitType _fitType;

    public StreamPlayerControl()
    {
        InitializeComponent();
        PaintSurface += (_, e) => SkiaHelper.SkControlOnPaintSurface(e, _currentFrame, _fitType);
        _player = new StreamPlayer(RtspTransport.Tcp);
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