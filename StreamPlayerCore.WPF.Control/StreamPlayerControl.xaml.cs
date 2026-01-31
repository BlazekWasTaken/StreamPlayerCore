using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Microsoft.Extensions.DependencyInjection;
using StreamPlayerCore.Enums;

namespace StreamPlayerCore.WPF.Control;

public delegate void StreamStarted();

public delegate void StreamStopped(StreamStopReason reason);

public class PlayerOptions
{
    public RtspTransport Transport { get; set; } = RtspTransport.Tcp;
    public RtspFlags Flags { get; set; } = RtspFlags.None;
    public int AnalyzeDuration { get; set; } = 0;
    public int ProbeSize { get; set; } = 65536;
}

public partial class StreamPlayerControl
{
    // ReSharper disable once MemberCanBePrivate.Global
    public PlayerOptions Options { get; } = new();
    
    private readonly StreamPlayer _player;

    public StreamPlayerControl(IServiceScopeFactory serviceScopeFactory)
    {
        InitializeComponent();
        
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
        _player?.Stop();
    }

    [DllImport("gdi32.dll")]
    private static extern bool DeleteObject(IntPtr hObject);

    private void Player_FrameReadyEvent(Bitmap frame)
    {
        Dispatcher.Invoke(() =>
        {
            var width = (int)Math.Round(ActualWidth);
            var height = (int)Math.Round(ActualHeight);
            var hBitmap = frame.GetHbitmap();
            try
            {
                var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromWidthAndHeight(width, height));
                Image.Source = bitmapSource;
            }
            finally
            {
                DeleteObject(hBitmap);
                frame.Dispose();
            }
        });
    }
}