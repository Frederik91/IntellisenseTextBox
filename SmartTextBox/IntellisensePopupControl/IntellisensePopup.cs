using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using SmartTextBox.Controls;
using SmartTextBox.Models;

namespace SmartTextBox.IntellisensePopupControl
{
    public class IntellisensePopup : Control
    {
        public static readonly DependencyProperty PopupPlacementTargetProperty = DependencyProperty.Register(
            "PopupPlacementTarget", typeof(UIElement), typeof(IntellisensePopup), new PropertyMetadata(default(UIElement)));

        public UIElement PopupPlacementTarget
        {
            get { return (UIElement)GetValue(PopupPlacementTargetProperty); }
            set { SetValue(PopupPlacementTargetProperty, value); }
        }

        public static readonly DependencyProperty PopupPlacementRectangleProperty = DependencyProperty.Register(
            "PopupPlacementRectangle", typeof(Rect), typeof(IntellisensePopup), new PropertyMetadata(default(Rect)));

        public Rect PopupPlacementRectangle
        {
            get { return (Rect)GetValue(PopupPlacementRectangleProperty); }
            set { SetValue(PopupPlacementRectangleProperty, value); }
        }


        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
            "IsOpen", typeof(bool), typeof(IntellisensePopup), new PropertyMetadata(default(bool)));

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        private ListView _itemsListView;
        private Popup _popup;
        private IEnumerable<GroupStyle> _groupStyles;
        private bool _disableItemSelectedFlag;

        public Func<string, List<object>> SearchFunction { get; set; }
        public Action<object> ItemSelectedAction { get; set; }

        static IntellisensePopup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IntellisensePopup), new FrameworkPropertyMetadata(typeof(IntellisensePopup)));
        }

        public void UpdateKeyDown(KeyEventArgs e)
        {
            OnKeyDown(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (IsOpen && _itemsListView != null)
            {
                try
                {
                    _disableItemSelectedFlag = true;
                    var count = _itemsListView.ItemsSource.OfType<object>().Count();
                    if (e.Key == Key.Down && _itemsListView.SelectedIndex < count - 1)
                        _itemsListView.SelectedIndex++;
                    else if (e.Key == Key.Up && _itemsListView.SelectedIndex > 0)
                        _itemsListView.SelectedIndex--;

                    if (count == 0)
                        IsOpen = false;
                }
                finally
                {
                    _disableItemSelectedFlag = false;
                }
            }


            base.OnKeyDown(e);
        }

        public override void OnApplyTemplate()
        {
            _itemsListView = Template.FindName("PART_ItemsListView", this) as ListView;
            _itemsListView.SelectionChanged += (s, e) => SelectionChanged();
            UpdateGroupStyle(_groupStyles);
            _popup = Template.FindName("PART_Popup", this) as Popup;
            SubscribeToMoveWithWindow();
            base.OnApplyTemplate();
        }

        private void SelectionChanged()
        {
            if (_itemsListView?.SelectedItem is null || ItemSelectedAction is null || _disableItemSelectedFlag)
                return;

            ItemSelectedAction?.Invoke(_itemsListView.SelectedItem);
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

        public void Show(Rect rect)
        {
            PopupPlacementRectangle = rect;
            IsOpen = true;
        }

        public void Close()
        {
            IsOpen = false;
        }

        public object GetSelectedItem()
        {
            return _itemsListView.SelectedItem;
        }

        public void UpdateGroupStyle(IEnumerable<GroupStyle> groupStyle)
        {
            if (_itemsListView is null)
            {
                _groupStyles = groupStyle;
                return;
            }

            _itemsListView.GroupStyle.Clear();
            foreach (var style in groupStyle)
                _itemsListView.GroupStyle.Add(style);
        }
    }
}
