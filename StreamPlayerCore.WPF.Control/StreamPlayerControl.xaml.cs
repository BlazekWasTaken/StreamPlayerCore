using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using FFmpeg.AutoGen;
using Microsoft.Extensions.Logging;
using StreamPlayerCore.Helper;

namespace StreamPlayerCore.WPF.Control;

public delegate void StreamStarted();

public delegate void StreamStopped(StreamStopReason reason);

public partial class StreamPlayerControl
{
    private readonly StreamPlayer _player;
    private readonly TimeSpan _timeout;

    public StreamPlayerControl(ILoggerFactory loggerFactory, TimeSpan timeout,
        RtspTransport transport = RtspTransport.Tcp, RtspFlags flags = RtspFlags.None,
        int analyzeDuration = 0, int probeSize = 65536,
        AVHWDeviceType hwDeviceType = AVHWDeviceType.AV_HWDEVICE_TYPE_NONE,
        FFmpegLogLevel ffmpegLogLevel = FFmpegLogLevel.AvLogDebug)
    {
        InitializeComponent();
        _timeout = timeout;
        _player = new StreamPlayer(loggerFactory,
            transport,
            flags,
            analyzeDuration,
            probeSize,
            hwDeviceType,
            (int)ffmpegLogLevel);
        _player.FrameReadyEvent += Player_FrameReadyEvent;
        _player.StreamStartedEvent += () => { StreamStartedEvent?.Invoke(); };
        _player.StreamStoppedEvent += reason => { StreamStoppedEvent?.Invoke(reason); };
    }

    public event StreamStarted? StreamStartedEvent;
    public event StreamStopped? StreamStoppedEvent;

    public void StartStream(string url)
    {
        _player.Start(new Uri(url), _timeout);
    }

    public void StopStream()
    {
        _player.Stop();
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