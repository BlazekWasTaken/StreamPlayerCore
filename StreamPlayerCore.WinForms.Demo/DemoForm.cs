using StreamPlayerCore.WinForms.Control;

namespace StreamPlayerCore.WinForms.Demo;

public partial class DemoForm : Form
{
    public DemoForm()
    {
        InitializeComponent();
        streamPlayerControl2.StreamFailed += (_, args) => Console.WriteLine(args.Error);
        streamPlayerControl2.StreamStarted += (_, _) => Console.WriteLine("stream started");
        streamPlayerControl2.StreamStopped += (_, _) => Console.WriteLine("stream stopped");
    }

    private void button1_Click(object sender, EventArgs e)
    {
        streamPlayerControl2.StartPlay(new Uri(textBox1.Text),
            TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(3.0),
            RtspTransport.Tcp, RtspFlags.None, int.Parse(textBox3.Text), int.Parse(textBox2.Text));
    }

    private void button2_Click(object sender, EventArgs e)
    {
        if (streamPlayerControl2.IsPlaying) streamPlayerControl2.Stop();
    }
}