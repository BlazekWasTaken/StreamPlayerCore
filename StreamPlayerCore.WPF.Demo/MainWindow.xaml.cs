using System.Windows;
using Microsoft.Extensions.Logging;
using StreamPlayerCore.WPF.Control;

namespace StreamPlayerCore.WPF.Demo;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private readonly ILoggerFactory _loggerFactory;
    private StreamPlayerControl? _player1;
    private StreamPlayerControl? _player2;

    public MainWindow(ILoggerFactory loggerFactory)
    {
        InitializeComponent();
        _loggerFactory = loggerFactory;
    }

    private void BtnStart1_OnClick(object sender, RoutedEventArgs e)
    {
        if (_player1 != null) return;
        var rtspUrl = TbUrl1.Text;
        _player1 = new StreamPlayerControl(_loggerFactory, TimeSpan.FromSeconds(5));
        _player1.StreamStartedEvent += () => { };
        _player1.StreamStoppedEvent += reason =>
        {
            MessageBox.Show($"stream stopped, {reason}");
            DpPlayer1.Children.Remove(_player1);
            _player1 = null;
        };
        DpPlayer1.Children.Add(_player1);
        _player1.StartStream(rtspUrl);
    }

    private void BtnStop1_OnClick(object sender, RoutedEventArgs e)
    {
        _player1?.StopStream();
    }

    private void BtnStart2_OnClick(object sender, RoutedEventArgs e)
    {
        if (_player2 != null) return;
        var rtspUrl = TbUrl2.Text;
        _player2 = new StreamPlayerControl(_loggerFactory, TimeSpan.FromSeconds(5));
        _player2.StreamStartedEvent += () => { };
        _player2.StreamStoppedEvent += reason =>
        {
            MessageBox.Show($"stream stopped, {reason}");
            DpPlayer2.Children.Remove(_player2);
            _player2 = null;
        };
        DpPlayer2.Children.Add(_player2);
        _player2.StartStream(rtspUrl);
    }

    private void BtnStop2_OnClick(object sender, RoutedEventArgs e)
    {
        _player2?.StopStream();
    }
}