using Microsoft.Extensions.Logging;
using StreamPlayerCore.WinForms.Control;

namespace StreamPlayerCore.WinForms.Demo;

public partial class DemoForm : Form
{
    private StreamPlayerControl? _player1;
    private StreamPlayerControl? _player2;

    private ILoggerFactory _loggerFactory;

    public DemoForm(ILoggerFactory loggerFactory)
    {
        InitializeComponent();
        _loggerFactory = loggerFactory;
    }

    private void btnStart1_Click(object sender, EventArgs e)
    {
        if (_player1 != null) return;
        var rtspUrl = tbUrl1.Text;
        _player1 = new StreamPlayerControl(_loggerFactory);
        _player1.Dock = DockStyle.Fill;
        panel1.Controls.Add(_player1);
        _player1.StartStream(rtspUrl);
    }

    private void btnStop1_Click(object sender, EventArgs e)
    {
        if (_player1 == null) return;
        _player1.StopStream();
        panel1.Controls.Remove(_player1);
        _player1.Dispose();
        _player1 = null;
    }

    private void btnStart2_Click(object sender, EventArgs e)
    {
        if (_player2 != null) return;
        var rtspUrl = tbUrl2.Text;
        _player2 = new StreamPlayerControl(_loggerFactory);
        _player2.Dock = DockStyle.Fill;
        panel2.Controls.Add(_player2);
        _player2.StartStream(rtspUrl);
    }

    private void btnStop2_Click(object sender, EventArgs e)
    {
        if (_player2 == null) return;
        _player2.StopStream();
        panel2.Controls.Remove(_player2);
        _player2.Dispose();
        _player2 = null;
    }
}