using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using StreamPlayerCore.WPF.Control;

namespace StreamPlayerCore.WPF.Demo;

/// <summary>
/// Interaction logic for MainWindow.xaml
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
            TimeSpan.FromSeconds(5.0), TimeSpan.FromSeconds(5.0),
            RtspTransport.Tcp, RtspFlags.None);
    }
}