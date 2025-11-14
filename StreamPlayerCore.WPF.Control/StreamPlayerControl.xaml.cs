using SkiaSharp;
using SkiaSharp.Views.Desktop;
using StreamPlayerCore.Helper;

namespace StreamPlayerCore.WPF.Control;

public partial class StreamPlayerControl
{
    private readonly StreamPlayer _player;
    private SKBitmap? _currentFrame;
    private FitType _fitType;
    
    public StreamPlayerControl()
    {
        InitializeComponent();
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