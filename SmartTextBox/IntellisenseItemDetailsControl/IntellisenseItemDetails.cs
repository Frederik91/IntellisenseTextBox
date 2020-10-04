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

        private Popup _popup;

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
            _popup = Template.FindName("PART_Popup", this) as Popup;
            _popup.Placement = PlacementMode.Top;
            SubscribeToMoveWithWindow();
            base.OnApplyTemplate();
        }

        private void SubscribeToMoveWithWindow()
        {
            var w = Window.GetWindow(this);
            // w should not be Null now!
            if (null != w)
            {
                w.LocationChanged += delegate
                {
                    var offset = _popup.HorizontalOffset;
                    // "bump" the offset to cause the popup to reposition itself
                    //   on its own
                    _popup.HorizontalOffset = offset + 1;
                    _popup.HorizontalOffset = offset;
                };
                // Also handle the window being resized (so the popup's position stays
                //  relative to its target element if the target element moves upon 
                //  window resize)
                w.SizeChanged += delegate
                {
                    var offset = _popup.HorizontalOffset;
                    _popup.HorizontalOffset = offset + 1;
                    _popup.HorizontalOffset = offset;
                };
            }
        }

        public void Close()
        {
            if (!IsMouseOver)
                IsOpen = false;
        }
    }
}
