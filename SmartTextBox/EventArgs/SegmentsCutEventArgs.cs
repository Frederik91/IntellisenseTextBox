using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using SmartTextBox.Models;

namespace SmartTextBox.EventArgs
{
    public class SegmentsCutEventArgs : RoutedEventArgs
    {
        public List<SegmentBase> CutSegments { get; set; }

        public SegmentsCutEventArgs()
        {
        }

        public SegmentsCutEventArgs(RoutedEvent routedEvent) : base(routedEvent)
        {
        }

        public SegmentsCutEventArgs(RoutedEvent routedEvent, object source) : base(routedEvent, source)
        {
        }
    }
}