using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace StreamPlayerCore.Helper;

public static class SkiaHelper
{
    public static void SkControlOnPaintSurface(SKPaintSurfaceEventArgs e, SKBitmap? currentFrame, FitType fitType)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.Black);

        if (currentFrame == null) return;
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (fitType)
        {
            case FitType.Center:
                DrawCenter(canvas, e, currentFrame);
                break;
            case FitType.Stretch:
                DrawStretch(canvas, e, currentFrame);
                break;
        }
    }

    private static void DrawStretch(SKCanvas canvas, SKPaintSurfaceEventArgs e, SKBitmap currentFrame)
    {
        var destRect = SKRect.Create(0, 0, e.Info.Width, e.Info.Height);
        using var image = SKImage.FromBitmap(currentFrame);
        if (image == null) return;
        canvas.DrawImage(image, destRect);
    }

    private static void DrawCenter(SKCanvas canvas, SKPaintSurfaceEventArgs e, SKBitmap currentFrame)
    {
        var controlAspect = (float)e.Info.Width / e.Info.Height;
        var imageAspect = (float)currentFrame.Width / currentFrame.Height;
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

        using var image = SKImage.FromBitmap(currentFrame);
        if (image == null) return;
        canvas.DrawImage(image, destRect);
    }
}