using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace StreamPlayerCore.WinForms.Control;

public enum FitType
{
    Stretch,
    Center
}

public partial class StreamPlayerControl : SKControl
{
    private readonly StreamPlayer _player;
    private SKBitmap? _currentFrame;
    private FitType _fitType;
    
    public StreamPlayerControl()
    {
        InitializeComponent();
        PaintSurface += SkControlOnPaintSurface;
        _player = new StreamPlayer(@"C:\Users\blazej\Desktop\ffmpeg-8.0-full_build-shared\bin", RtspTransport.Tcp);
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
    
    private void SkControlOnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.Black);

        if (_currentFrame == null) return;
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (_fitType)
        {
            case FitType.Center:
                DrawCenter(canvas, e);
                break;
            case FitType.Stretch:
                DrawStretch(canvas, e);
                break;
        }
    }

    private void DrawStretch(SKCanvas canvas, SKPaintSurfaceEventArgs e)
    {
        var destRect = SKRect.Create(0, 0, e.Info.Width, e.Info.Height);
        canvas.DrawBitmap(_currentFrame, destRect);
    }

    private void DrawCenter(SKCanvas canvas, SKPaintSurfaceEventArgs e)
    {
        var destRect = SKRect.Create(
            (float)(e.Info.Width - _currentFrame!.Width) / 2,
            (float)(e.Info.Height - _currentFrame.Height) / 2,
            _currentFrame.Width,
            _currentFrame.Height);
        canvas.DrawBitmap(_currentFrame, destRect);
    }
}