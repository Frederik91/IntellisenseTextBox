using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using SmartTextBox.IntellisenseItemControl;
using SmartTextBox.Models;

namespace SmartTextBox.Controls
{
    /// <summary>
    /// Interaction logic for IntellisenseTextBoxControl.xaml
    /// </summary>
    public partial class IntellisenseTextBox : UserControl
    {
        public static readonly DependencyProperty SearchFunctionProperty = DependencyProperty.Register(
    "SearchFunction", typeof(Func<string, List<object>>), typeof(IntellisenseTextBox), new PropertyMetadata(default(Func<string, List<object>>)));

        public Func<string, List<object>> SearchFunction
        {
            get { return (Func<string, List<object>>)GetValue(SearchFunctionProperty); }
            set { SetValue(SearchFunctionProperty, value); if (IntellisensePopup != null) IntellisensePopup.SearchFunction = SearchFunction; }
        }

        public static readonly DependencyProperty SegmentsProperty = DependencyProperty.Register(
            "Segments", typeof(List<SegmentBase>), typeof(IntellisenseTextBox), new PropertyMetadata(default(List<SegmentBase>), OnSegmentsChanged));

        public List<SegmentBase> Segments
        {
            get { return (List<SegmentBase>)GetValue(SegmentsProperty); }
            set { SetValue(SegmentsProperty, value); }
        }


        public static readonly DependencyProperty ListItemTemplateProperty = DependencyProperty.Register(
            "ListItemTemplate", typeof(DataTemplate), typeof(IntellisenseTextBox), new PropertyMetadata(default(DataTemplate), null, CoerceValueCallback));

        public DataTemplate ListItemTemplate
        {
            get { return (DataTemplate) GetValue(ListItemTemplateProperty); }
            set { SetValue(ListItemTemplateProperty, value); }
        }


        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
            "ItemTemplate", typeof(DataTemplate), typeof(IntellisenseTextBox), new PropertyMetadata(null, null, CoerceValueCallback));

        private static object CoerceValueCallback(DependencyObject d, object baseValue)
        {
            if (!(baseValue is DataTemplate dataTemplate))
                return null;

            return dataTemplate.HasContent ? dataTemplate : null;
        }

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        private static void OnSegmentsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is IntellisenseTextBox intellisenseTextBox))
                return;
            intellisenseTextBox.OnSegmentsChanged();
        }

        private void OnSegmentsChanged()
        {
            if (RichTextBox is null)
                return;

            var paragraph = GetParagraph();
            paragraph.Inlines.Clear();

            foreach (var segment in Segments ?? new List<SegmentBase>())
            {
                switch (segment)
                {
                    case TextSegment textSegment:
                        paragraph.Inlines.Add(textSegment.Text);
                        break;
                    case ObjectSegment objectSegment:
                        paragraph.Inlines.Add(new IntellisenseItem { Content = objectSegment.Content });
                        break;
                }
            }
        }

        private Paragraph GetParagraph()
        {
            return RichTextBox.Document.Blocks.FirstBlock as Paragraph;
        }


        private void FormatSuggestions()
        {
            var characterAtCarrot = RichTextBox.CaretPosition.GetPointerContext(LogicalDirection.Backward);
            if (characterAtCarrot != TextPointerContext.Text)
                return;

            var start = RichTextBox.CaretPosition;

            var stringBeforeCaret = start.GetTextInRun(LogicalDirection.Backward);

            if (!stringBeforeCaret.Contains("@"))
            {
                IntellisensePopup.Close();
                return;
            }

            var searchText = GetSearchText(stringBeforeCaret);
            IntellisensePopup.Search(searchText);

            var rect = RichTextBox.CaretPosition.GetCharacterRect(LogicalDirection.Backward);
            IntellisensePopup.Show(rect);
        }

        private void OnTextBoxKeyUp(KeyEventArgs e)
        {
            if (IntellisensePopup?.IsOpen != true)
            {
                base.OnKeyDown(e);
                return;
            }


            IntellisensePopup.UpdateKeyDown(e);

            if (e.Key == Key.Enter)
            {
                InsertItem(IntellisensePopup.GetSelectedItem());
            }

        }

        private void InsertItem(object suggestion)
        {
            var start = RichTextBox.CaretPosition;
            if (!(start.Parent is Run run))
                return;

            var stringBeforeCaret = start.GetTextInRun(LogicalDirection.Backward);
            var textAfterCaret = start.GetTextInRun(LogicalDirection.Forward);


            if (run.Text.EndsWith("@"))
                run.Text = run.Text.Remove(stringBeforeCaret.Length - 1, 1) + textAfterCaret;
            var newItem = new InlineUIContainer(new IntellisenseItem { Content = suggestion });
            start.Paragraph.Inlines.InsertAfter(run, newItem);
            IntellisensePopup.Close();
            RichTextBox.CaretPosition = newItem.ContentEnd;
        }

        private string GetSearchText(string stringBeforeCaret)
        {
            if (stringBeforeCaret.EndsWith("@"))
                return string.Empty;

            return stringBeforeCaret.Remove(0, stringBeforeCaret.IndexOf("@", StringComparison.InvariantCulture) + 3).TrimStart();
        }

        public IntellisenseTextBox()
        {
            InitializeComponent();
            IntellisensePopup.PopupPlacementTarget = this;

            RichTextBox.KeyUp += (s, e) =>
            {
                FormatSuggestions();
                OnTextBoxKeyUp(e);
            };
            IntellisensePopup.SearchFunction = SearchFunction;
            OnSegmentsChanged();
        }
    }
}
