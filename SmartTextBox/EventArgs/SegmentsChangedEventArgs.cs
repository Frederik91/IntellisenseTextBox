using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using SmartTextBox.Models;

namespace SmartTextBox.EventArgs
{
    public class SegmentsChangedEventArgs : RoutedEventArgs
    {
        public List<SegmentBase> Segments { get; set; }

        public SegmentsChangedEventArgs()
        {
        }

        public SegmentsChangedEventArgs(RoutedEvent routedEvent) : base(routedEvent)
        {
        }

        public SegmentsChangedEventArgs(RoutedEvent routedEvent, object source) : base(routedEvent, source)
        {
        }
    }
}