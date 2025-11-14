# StreamPlayerCore.WinForms.Control

[![NuGet](https://img.shields.io/badge/NuGet-StreamPlayerCore.WinForms.Control-004880?logo=nuget&logoColor=fff)](https://www.nuget.org/packages/StreamPlayerCore.WinForms.Control/)

WinForms control for video streaming using FFmpeg.

Built with .NET 10.

## Installation

You can install the StreamPlayerCore WinForms control via NuGet Package Manager:

```
Install-Package StreamPlayerCore.WinForms.Control
```

## Usage

For a complete example of using the StreamPlayerCore WinForms control, please refer to the [StreamPlayerCore.WinForms.Demo](https://github.com/BlazekWasTaken/StreamPlayerCore/tree/main/StreamPlayerCore.WinForms.Demo) project.

To use the StreamPlayerCore WinForms control, it is recommended to add a Panel to your window and place the StreamPlayerCore control inside it programatically.

```csharp
using StreamPlayerCore.WinForms.Control;

namespace StreamPlayerCore.WinForms.Demo
{
    public partial class MainForm : Form
    {
        private StreamPlayerControl _streamPlayer;

        public MainForm()
        {
            InitializeComponent();

            _streamPlayer = new StreamPlayerControl();
            _streamPlayer.Dock = DockStyle.Fill;
            panelPlayer1.Controls.Add(_streamPlayer);
            _streamPlayer.StartStream("rtsp://your_stream_url");
        }
    }
}
```

## License

This project is licensed under the LGPLv3 License. See the [LICENSE](https://github.com/BlazekWasTaken/StreamPlayerCore/blob/main/LICENSE) file for details.