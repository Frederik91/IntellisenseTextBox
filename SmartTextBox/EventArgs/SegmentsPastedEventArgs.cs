using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using SmartTextBox.Models;

namespace SmartTextBox.EventArgs
{
    public class SegmentsPastedEventArgs : RoutedEventArgs
    {
        public List<SegmentBase> PastedSegments { get; set; }

        public SegmentsPastedEventArgs()
        {
        }

        public SegmentsPastedEventArgs(RoutedEvent routedEvent) : base(routedEvent)
        {
        }

        public SegmentsPastedEventArgs(RoutedEvent routedEvent, object source) : base(routedEvent, source)
        {
        }
    }
}