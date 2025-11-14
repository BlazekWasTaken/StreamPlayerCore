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
        using var image = SKImage.FromBitmap(_currentFrame!);
        if (image == null) return;
        canvas.DrawImage(image, destRect);
    }

    private void DrawCenter(SKCanvas canvas, SKPaintSurfaceEventArgs e)
    {
        var controlAspect = (float)e.Info.Width / e.Info.Height;
        var imageAspect = (float)_currentFrame!.Width / _currentFrame.Height;
        SKRect destRect;
        if (imageAspect > controlAspect)
        {
            var scaledHeight = e.Info.Width / imageAspect;
            var yOffset = (e.Info.Height - scaledHeight) / 2;
            destRect = SKRect.Create(0, yOffset, e.Info.Width, scaledHeight);
        }
        else
        {
            var scaledWidth = e.Info.Height * imageAspect;
            var xOffset = (e.Info.Width - scaledWidth) / 2;
            destRect = SKRect.Create(xOffset, 0, scaledWidth, e.Info.Height);
        }
        
        using var image = SKImage.FromBitmap(_currentFrame);
        if (image == null) return;
        canvas.DrawImage(image, destRect);
    }
}