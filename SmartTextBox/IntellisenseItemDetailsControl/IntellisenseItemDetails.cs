using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using SmartTextBox.IntellisenseItemControl;

namespace SmartTextBox.IntellisenseItemDetailsControl
{
    public class IntellisenseItemDetails : Control
    {
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
            "IsOpen", typeof(bool), typeof(IntellisenseItemDetails), new PropertyMetadata(default(bool)));

        public bool IsOpen
        {
            get { return (bool) GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public static readonly DependencyProperty PopupPlacementTargetProperty = DependencyProperty.Register(
            "PopupPlacementTarget", typeof(UIElement), typeof(IntellisenseItemDetails), new PropertyMetadata(default(UIElement)));

        public UIElement PopupPlacementTarget
        {
            get { return (UIElement)GetValue(PopupPlacementTargetProperty); }
            set { SetValue(PopupPlacementTargetProperty, value); }
        }

        public static readonly DependencyProperty PopupPlacementRectangleProperty = DependencyProperty.Register(
            "PopupPlacementRectangle", typeof(Rect), typeof(IntellisenseItemDetails), new PropertyMetadata(default(Rect)));

        public Rect PopupPlacementRectangle
        {
            get { return (Rect)GetValue(PopupPlacementRectangleProperty); }
            set { SetValue(PopupPlacementRectangleProperty, value); }
        }

        static IntellisenseItemDetails()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IntellisenseItemDetails), new FrameworkPropertyMetadata(typeof(IntellisenseItemDetails)));
        }

        public override void OnApplyTemplate()
        {
            var popup = Template.FindName("PART_Popup", this) as Popup;
            popup.MouseLeave += PopupOnMouseLeave;
            base.OnApplyTemplate();
        }

        private void PopupOnMouseLeave(object sender, MouseEventArgs e)
        {
            
        }

        public void Close()
        {
            if (!IsMouseOver)
                IsOpen = false;
        }
    }
}
