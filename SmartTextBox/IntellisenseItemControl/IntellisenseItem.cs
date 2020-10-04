using System.Windows;
using System.Windows.Controls;

namespace SmartTextBox.IntellisenseItemControl
{
    public class IntellisenseItem : Control
    {
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected", typeof(bool), typeof(IntellisenseItem), new PropertyMetadata(default(bool)));

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content", typeof(object), typeof(IntellisenseItem), new PropertyMetadata(default(object)));

        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        static IntellisenseItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IntellisenseItem), new FrameworkPropertyMetadata(typeof(IntellisenseItem)));
        }
    }
}
