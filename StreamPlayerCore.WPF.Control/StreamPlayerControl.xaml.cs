using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using FFmpeg.AutoGen;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StreamPlayerCore.Enums;
using StreamPlayerCore.Helper;

namespace StreamPlayerCore.WPF.Control;

public delegate void StreamStarted();

public delegate void StreamStopped(StreamStopReason reason);

public class PlayerOptions
{
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);
    public RtspTransport Transport { get; set; } = RtspTransport.Tcp;
    public RtspFlags Flags { get; set; } = RtspFlags.None;
    public int AnalyzeDuration { get; set; } = 0;
    public int ProbeSize { get; set; } = 65536;
    public AVHWDeviceType HwDeviceType { get; set; } = AVHWDeviceType.AV_HWDEVICE_TYPE_NONE; // TODO: IOptions
    public FFmpegLogLevel FfmpegLogLevel = FFmpegLogLevel.AvLogDebug; // TODO: IOptions
}

public partial class StreamPlayerControl
{
    private readonly IServiceProvider _serviceProvider;
    
    private PlayerOptions _options = new();
    
    private StreamPlayer? _player;

    public StreamPlayerControl(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
    }
    
    public void Configure(PlayerOptions options)
    {
        _options = options;
    }

    public event StreamStarted? StreamStartedEvent;
    public event StreamStopped? StreamStoppedEvent;

    public void StartStream(string url)
    {
        _player = _serviceProvider.GetRequiredService<StreamPlayer>();
        
        _player.FrameReadyEvent += Player_FrameReadyEvent;
        _player.StreamStartedEvent += () => { StreamStartedEvent?.Invoke(); };
        _player.StreamStoppedEvent += reason => { StreamStoppedEvent?.Invoke(reason); };
        
        _player.Start(new Uri(url), _options.Timeout);
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