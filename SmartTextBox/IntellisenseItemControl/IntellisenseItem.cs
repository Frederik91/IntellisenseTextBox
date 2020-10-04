using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SmartTextBox.IntellisenseItemDetailsControl;

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


        private IntellisenseItemDetails _itemDetails;

        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        static IntellisenseItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IntellisenseItem), new FrameworkPropertyMetadata(typeof(IntellisenseItem)));
        }

        public override void OnApplyTemplate()
        {
            _itemDetails = Template.FindName("PART_ItemDetails", this) as IntellisenseItemDetails;
            Window.GetWindow(this).MouseDown += (s, e) => CloseDetails();
            base.OnApplyTemplate();
        }

        public void ShowDetails(Rect rectangle)
        {
            _itemDetails.IsOpen = true;
            _itemDetails.Focus();
            rectangle.Offset(100,100);
            _itemDetails.PopupPlacementRectangle = rectangle;
            _itemDetails.PopupPlacementTarget = this;
        }

        public void CloseDetails()
        {
            _itemDetails.Close();
        }
    }
}
