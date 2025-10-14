using System.Windows;
using StreamPlayerCore.WPF.Control;

namespace StreamPlayerCore.WPF.Demo;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        StreamPlayerControl.StreamFailed += (_, args) => Console.WriteLine(args.Error);
        StreamPlayerControl.StreamStarted += (_, _) => Console.WriteLine("stream started");
        StreamPlayerControl.StreamStopped += (_, _) => Console.WriteLine("stream stopped");
    }

    private void Button1_OnClick(object sender, RoutedEventArgs e)
    {
        if (StreamPlayerControl.IsPlaying) StreamPlayerControl.Stop();
    }

    private void Button2_OnClick(object sender, RoutedEventArgs e)
    {
        StreamPlayerControl.StartPlay(new Uri(TextBox1.Text),
            TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(3.0),
            RtspTransport.Tcp, RtspFlags.None, int.Parse(TextBox2.Text), int.Parse(TextBox3.Text));
    }
}