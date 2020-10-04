using System.Windows;

namespace SmartTextBox.EventArgs
{
    public class SearchChangedEventArgs : RoutedEventArgs
    {
        public string SearchText { get; set; }

        public SearchChangedEventArgs()
        {
        }

        public SearchChangedEventArgs(RoutedEvent routedEvent) : base(routedEvent)
        {
        }

        public SearchChangedEventArgs(RoutedEvent routedEvent, object source) : base(routedEvent, source)
        {
        }
    }
}