using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using SmartTextBox.Models;

namespace SmartTextBox.EventArgs
{
    public class SegmentsCopiedEventArgs : RoutedEventArgs
    {
        public List<SegmentBase> CopiedSegments { get; set; }

        public SegmentsCopiedEventArgs()
        {
        }

        public SegmentsCopiedEventArgs(RoutedEvent routedEvent) : base(routedEvent)
        {
        }

        public SegmentsCopiedEventArgs(RoutedEvent routedEvent, object source) : base(routedEvent, source)
        {
        }
    }
}