# StreamPlayerCore.WPF.Control

[![NuGet](https://img.shields.io/badge/NuGet-StreamPlayerCore.WPF.Control-004880?logo=nuget&logoColor=fff)](https://www.nuget.org/packages/StreamPlayerCore.WPF.Control/)

WPF control for video streaming using FFmpeg.

Built with .NET 10.

## Installation

You can install the StreamPlayerCore WPF control via NuGet Package Manager:

```
Install-Package StreamPlayerCore.WPF.Control
```

## Usage

For a complete example of using the StreamPlayerCore WPF control, please refer to
the [StreamPlayerCore.WPF.Demo](../StreamPlayerCore.WPF.Demo) project.

To use the StreamPlayerCore WPF control, it is recommended to add a DockPanel to your window and place the
StreamPlayerCore control inside it programatically.

```xml
<Window x:Class="StreamPlayerCore.WPF.Demo.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        mc:Ignorable="d"
        Title="MainWindow" Height="610" Width="1330">
    <DockPanel x:Name="DpPlayer1"/>
</Window>
```

```csharp
using StreamPlayerCore.WPF.Control;
using System.Windows;

namespace StreamPlayerCore.WPF.Demo
{
    public partial class MainWindow : Window
    {
        private StreamPlayerControl _streamPlayer;

        public MainWindow()
        {
            InitializeComponent();

            _streamPlayer = new StreamPlayerControl();
            DpPlayer1.Children.Add(_streamPlayerWPF);
            _streamPlayer.StartStream("rtsp://your_stream_url");
        }
    }
}
```

## License

This project is licensed under the LGPLv3 License. See the [LICENSE](../LICENSE) file for details.