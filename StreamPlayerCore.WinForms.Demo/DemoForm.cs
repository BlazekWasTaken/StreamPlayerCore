using Microsoft.Extensions.DependencyInjection;
using StreamPlayerCore.WinForms.Control;

namespace StreamPlayerCore.WinForms.Demo;

public partial class DemoForm : Form
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    
    private StreamPlayerControl? _player1;
    private StreamPlayerControl? _player2;

    public DemoForm(IServiceScopeFactory serviceScopeFactory)
    {
        InitializeComponent();
        _serviceScopeFactory = serviceScopeFactory;

        ResizeBegin += (_, _) => SuspendLayout();
        ResizeEnd += (_, _) => ResumeLayout();
    }

    private void btnStart1_Click(object sender, EventArgs e)
    {
        if (_player1 != null) return;
        var rtspUrl = tbUrl1.Text;
        _player1 = new StreamPlayerControl(_serviceScopeFactory);
        // using var scope = _serviceScopeFactory.CreateScope();
        // _player1 = scope.ServiceProvider.GetRequiredService<StreamPlayerControl>(); // TODO: issue with DI in winforms
        _player1.StreamStartedEvent += () => { };
        _player1.StreamStoppedEvent += reason =>
        {
            MessageBox.Show($"stream stopped, {reason}");
            panel1.Controls.Remove(_player1);
            _player1.Dispose();
            _player1 = null;
        };
        _player1.Dock = DockStyle.Fill;
        panel1.Controls.Add(_player1);
        _player1.StartStream(rtspUrl);
    }

    private void btnStop1_Click(object sender, EventArgs e)
    {
        _player1?.StopStream();
    }

    private void btnStart2_Click(object sender, EventArgs e)
    {
        if (_player2 != null) return;
        var rtspUrl = tbUrl2.Text;
        _player2 = new StreamPlayerControl(_serviceScopeFactory);
        _player2.StreamStartedEvent += () => { };
        _player2.StreamStoppedEvent += reason =>
        {
            MessageBox.Show($"stream stopped, {reason}");
            panel2.Controls.Remove(_player2);
            _player2.Dispose();
            _player2 = null;
        };
        _player2.Dock = DockStyle.Fill;
        panel2.Controls.Add(_player2);
        _player2.StartStream(rtspUrl);
    }

    private void btnStop2_Click(object sender, EventArgs e)
    {
        _player2?.StopStream();
    }
}