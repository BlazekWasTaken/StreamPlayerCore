using System.Windows;

namespace StreamPlayerCore.WPF.Control;

public class StreamFailedEventArgs : RoutedEventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="StreamFailedEventArgs" /> class.
    /// </summary>
    /// <param name="routedEvent">
    ///     The routed event.
    /// </param>
    /// <param name="error">
    ///     The error message.
    /// </param>
    public StreamFailedEventArgs(RoutedEvent routedEvent,
        string error) : base(routedEvent)
    {
        Error = error;
    }

    /// <summary>
    ///     Gets the error message.
    /// </summary>
    public string Error { get; }
}