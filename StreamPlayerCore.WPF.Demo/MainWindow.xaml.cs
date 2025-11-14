using System.Windows;
using StreamPlayerCore.WPF.Control;

namespace StreamPlayerCore.WPF.Demo;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private StreamPlayerControl? _player1;
    private StreamPlayerControl? _player2;
    
    public MainWindow()
    {
        InitializeComponent();
    }

    private void BtnStart1_OnClick(object sender, RoutedEventArgs e)
    {
        if (_player1 != null) return;
        var rtspUrl = TbUrl1.Text;
        _player1 = new StreamPlayerControl();
        DpPlayer1.Children.Add(_player1);
        _player1.StartStream(rtspUrl);
    }
    
    private void BtnStop1_OnClick(object sender, RoutedEventArgs e)
    {
        if (_player1 == null) return;
        _player1.StopStream();
        DpPlayer1.Children.Remove(_player1);
        _player1 = null;
    }

    private void BtnStart2_OnClick(object sender, RoutedEventArgs e)
    {
        if (_player2 != null) return;
        var rtspUrl = TbUrl2.Text;
        _player2 = new StreamPlayerControl();
        DpPlayer2.Children.Add(_player2);
        _player2.StartStream(rtspUrl);
    }

    private void BtnStop2_OnClick(object sender, RoutedEventArgs e)
    {
        if (_player2 == null) return;
        _player2.StopStream();
        DpPlayer2.Children.Remove(_player2);
        _player2 = null;
    }
}