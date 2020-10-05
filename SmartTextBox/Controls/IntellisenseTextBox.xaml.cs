using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using Newtonsoft.Json;
using SmartTextBox.EventArgs;
using SmartTextBox.IntellisenseItemControl;
using SmartTextBox.Models;
using Clipboard = System.Windows.Clipboard;
using DataObject = System.Windows.DataObject;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace SmartTextBox.Controls
{
    public delegate void SearchChangedEventHandler(object sender, SearchChangedEventArgs args);
    public delegate void SegmentsChangedEventHandler(object sender, SegmentsChangedEventArgs args);

    public partial class IntellisenseTextBox
    {
        private static readonly RoutedEvent SearchChangedEvent = EventManager.RegisterRoutedEvent("SearchChanged", RoutingStrategy.Direct, typeof(SearchChangedEventHandler), typeof(IntellisenseTextBox));
        private static readonly RoutedEvent SegmentChangedEvent = EventManager.RegisterRoutedEvent("SegmentsChanged", RoutingStrategy.Direct, typeof(SegmentsChangedEventHandler), typeof(IntellisenseTextBox));

        public static readonly DependencyProperty SegmentsProperty = DependencyProperty.Register("Segments", typeof(List<SegmentBase>), typeof(IntellisenseTextBox), new PropertyMetadata(default(List<SegmentBase>), OnSegmentsChanged));

        public List<SegmentBase> Segments
        {
            get { return (List<SegmentBase>)GetValue(SegmentsProperty); }
            set { SetValue(SegmentsProperty, value); }
        }

        public static readonly DependencyProperty ListItemTemplateProperty = DependencyProperty.Register(
            "ListItemTemplate", typeof(DataTemplate), typeof(IntellisenseTextBox), new PropertyMetadata(default(DataTemplate), null, CoerceValueCallback));

        public DataTemplate ListItemTemplate
        {
            get { return (DataTemplate)GetValue(ListItemTemplateProperty); }
            set { SetValue(ListItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty DetailItemTemplateProperty = DependencyProperty.Register(
            "DetailItemTemplate", typeof(DataTemplate), typeof(IntellisenseTextBox), new PropertyMetadata(default(DataTemplate), null, CoerceValueCallback));

        public DataTemplate DetailItemTemplate
        {
            get { return (DataTemplate)GetValue(DetailItemTemplateProperty); }
            set { SetValue(DetailItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty GroupStyleProperty = DependencyProperty.Register(
            "GroupStyle", typeof(ObservableCollection<GroupStyle>), typeof(IntellisenseTextBox), new PropertyMetadata(new ObservableCollection<GroupStyle>()));

        public ObservableCollection<GroupStyle> GroupStyle => (ObservableCollection<GroupStyle>)GetValue(GroupStyleProperty);

        public static readonly DependencyProperty EnableDetailsProperty = DependencyProperty.Register(
            "EnableDetails", typeof(bool), typeof(IntellisenseTextBox), new PropertyMetadata(true));

        public bool EnableDetails
        {
            get { return (bool)GetValue(EnableDetailsProperty); }
            set { SetValue(EnableDetailsProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(IntellisenseTextBox), new PropertyMetadata(default(IEnumerable)));

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty SearchTextProperty = DependencyProperty.Register(
            "SearchText", typeof(string), typeof(IntellisenseTextBox), new PropertyMetadata(default(string), SearchTextPropertyChangedCallback));

        private static void SearchTextPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is IntellisenseTextBox intellisenseTextBox))
                return;

            intellisenseTextBox.RaiseSearchChangedEvent(e.NewValue?.ToString());
        }

        public static readonly DependencyProperty IntellisenseTriggerProperty = DependencyProperty.Register(
            "IntellisenseTrigger", typeof(string), typeof(IntellisenseTextBox), new PropertyMetadata("@"));

        public string IntellisenseTrigger
        {
            get { return (string)GetValue(IntellisenseTriggerProperty); }
            set { SetValue(IntellisenseTriggerProperty, value); }
        }


        private void RaiseSearchChangedEvent(string searchText)
        {
            RaiseEvent(new SearchChangedEventArgs(SearchChangedEvent, this) { SearchText = searchText });
        }

        private void RaiseSegmentsChangedEvent()
        {
            var segments = GetSegments();
            RaiseEvent(new SegmentsChangedEventArgs(SegmentChangedEvent, this) { Segments = segments });
        }

        public event SearchChangedEventHandler SearchChanged
        {
            add => AddHandler(SearchChangedEvent, value);
            remove => RemoveHandler(SearchChangedEvent, value);
        }

        public event SegmentsChangedEventHandler SegmentsChanged
        {
            add => AddHandler(SegmentChangedEvent, value);
            remove => RemoveHandler(SegmentChangedEvent, value);
        }

        public string SearchText
        {
            get { return (string)GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
            "ItemTemplate", typeof(DataTemplate), typeof(IntellisenseTextBox), new PropertyMetadata(null, null, CoerceValueCallback));

        private bool _disableSegmentRegen;

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

            if (!_disableSegmentRegen)
            {
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

                RichTextBox.CaretPosition = paragraph.ElementEnd;
            }

            RaiseSegmentsChangedEvent();
        }

        private List<SegmentBase> GetSegments()
        {
            var paragraph = GetParagraph();
            var segments = new List<SegmentBase>();
            if (paragraph?.Inlines is null)
                return segments;

            foreach (var inline in paragraph.Inlines)
            {
                if (inline is Run run)
                    segments.Add(new TextSegment { Text = run.Text });
                else if (inline is InlineUIContainer container && container.Child is IntellisenseItem item)
                    segments.Add(new ObjectSegment { Content = item.Content });
            }

            return segments;
        }

        private Paragraph GetParagraph()
        {
            return RichTextBox.Document.Blocks.FirstBlock as Paragraph;
        }


        private void EvaluateShowPopup(KeyEventArgs args)
        {
            var start = RichTextBox.CaretPosition;
            var stringBeforeCaret = start.GetTextInRun(LogicalDirection.Backward);
            if (!stringBeforeCaret.Contains(IntellisenseTrigger) || args.Key == Key.Escape)
            {
                IntellisensePopup.Close();
                return;
            }

            var searchText = GetSearchText(stringBeforeCaret);
            if (SearchText != searchText)
                SearchText = searchText;

            if (searchText.StartsWith(" "))
            {
                IntellisensePopup.Close();
                return;
            }

            var rect = RichTextBox.CaretPosition.GetCharacterRect(LogicalDirection.Backward);
            IntellisensePopup.Show(rect);
        }

        private void OnTextBoxKeyUp(KeyEventArgs e)
        {
            if (IntellisensePopup?.Popup.IsOpen != true)
            {
                EvaluateSegmentsChanged();
                base.OnKeyDown(e);
                return;
            }

            IntellisensePopup.UpdateKeyDown(e);

            if (e.Key == Key.Enter)
                InsertItem(IntellisensePopup.GetSelectedItem());

            EvaluateSegmentsChanged();
        }

        private void Copy()
        {
            var start = RichTextBox.Selection.Start;
            if (!(start.Parent is Inline currentInline))
                return;

            var end = RichTextBox.Selection.End;
            var endInline = RichTextBox.Selection.End.Parent as Inline;
            if (endInline is null && RichTextBox.Selection.End.Parent is Paragraph paragraph)
                endInline = paragraph.Inlines.LastInline;
            var startText = start.GetTextInRun(LogicalDirection.Forward);
            var segments = new List<SegmentBase>();
            if (!string.IsNullOrEmpty(startText))
                segments.Add(new TextSegment(startText));
            currentInline = currentInline.NextInline;
            while (currentInline != null)
            {
                if (currentInline == endInline)
                {
                    if (currentInline is Run)
                    {
                        var endText = end.GetTextInRun(LogicalDirection.Backward);
                        segments.Add(new TextSegment(endText));
                    }
                    else if (currentInline is InlineUIContainer container && container.Child is IntellisenseItem child)
                        segments.Add(new ObjectSegment { Content = child.Content });

                    break;
                }


                if (currentInline is Run run)
                    segments.Add(new TextSegment(run.Text));
                else if (currentInline is InlineUIContainer container && container.Child is IntellisenseItem child)
                    segments.Add(new ObjectSegment { Content = child.Content });


                currentInline = currentInline.NextInline;
            }

            var json = JsonConvert.SerializeObject(segments, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            Clipboard.SetText(json);
        }

        private void Paste()
        {
            if (!(RichTextBox?.Selection?.Start?.Paragraph?.Inlines is { } collection))
                return;

            RichTextBox.Selection.Text = string.Empty;

            try
            {
                var text = Clipboard.GetText();
                var segments = JsonConvert.DeserializeObject<List<SegmentBase>>(text, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
                if (segments is null)
                    return;

                Inline inline;
                var cutText = string.Empty;
                if (RichTextBox.Selection.Start.Parent is Run run)
                {
                    cutText = RichTextBox.Selection.Start.GetTextInRun(LogicalDirection.Forward);
                    run.Text = run.Text.Remove(run.Text.Length - cutText.Length, cutText.Length);
                    inline = run;
                }
                else if (RichTextBox.Selection.Start.Parent is Paragraph paragraph)
                    inline = paragraph.Inlines.LastInline;
                else
                    return;

                foreach (var segment in segments)
                {
                    Inline newInline;
                    switch (segment)
                    {
                        case TextSegment textSegment:
                            newInline = new Run(textSegment.Text);
                            break;
                        case ObjectSegment objectSegment:
                            newInline = new InlineUIContainer(new IntellisenseItem { Content = objectSegment.Content });
                            break;
                        default:
                            continue;
                    }

                    collection.InsertAfter(inline, newInline);
                    inline = newInline;
                }

                if (!string.IsNullOrEmpty(cutText))
                {
                    if (inline is Run lastRun)
                        lastRun.Text += cutText;
                    else
                        collection.InsertAfter(inline, new Run(cutText));
                }
            }
            catch (Exception)
            {
                // Ignored
            }


        }

        private void EvaluateSegmentsChanged()
        {
            var segments = GetSegments();
            try
            {
                _disableSegmentRegen = true;
                if (segments.Count != Segments.Count)
                {
                    Segments = segments;
                    return;
                }

                for (var i = 0; i < segments.Count; i++)
                {
                    var segment = segments[i];
                    var otherSegment = Segments[i];
                    if (segment.Equals(otherSegment))
                        continue;

                    Segments = segments;
                    return;
                }
            }
            finally
            {
                _disableSegmentRegen = false;
            }
        }

        private void InsertItem(object suggestion)
        {
            var start = RichTextBox.CaretPosition;
            if (!(start.Parent is Run run))
                return;

            var stringBeforeCaret = start.GetTextInRun(LogicalDirection.Backward);
            var textAfterCaret = start.GetTextInRun(LogicalDirection.Forward);

            var searchText = GetSearchText(stringBeforeCaret);
            if (run.Text.EndsWith(IntellisenseTrigger + searchText))
                run.Text = run.Text.Remove(stringBeforeCaret.Length - (1 + searchText.Length), 1 + searchText.Length) + textAfterCaret;
            var newItem = new InlineUIContainer(new IntellisenseItem { Content = suggestion });
            start.Paragraph.Inlines.InsertAfter(run, newItem);
            IntellisensePopup.Close();
            RichTextBox.CaretPosition = newItem.ContentEnd;
            RaiseSegmentsChangedEvent();
            SetFocusAfterDelay();
        }

        private async void SetFocusAfterDelay()
        {
            await Task.Delay(100);
            RichTextBox.Focus();
        }

        private string GetSearchText(string stringBeforeCaret)
        {
            if (stringBeforeCaret.EndsWith(IntellisenseTrigger))
                return string.Empty;

            return stringBeforeCaret.Remove(0, stringBeforeCaret.IndexOf(IntellisenseTrigger, StringComparison.InvariantCulture) + 1);
        }

        public IntellisenseTextBox()
        {
            GroupStyle.CollectionChanged += GroupStyleChanged;
            InitializeComponent();
            IntellisensePopup.PopupPlacementTarget = this;
            IntellisensePopup.UpdateGroupStyle(GroupStyle);
            IntellisensePopup.ItemSelectedAction = InsertItem;
            
            
            CommandManager.AddPreviewExecutedHandler(RichTextBox, PreviewExecuted);
            DataObject.AddPastingHandler(RichTextBox, PastCommand);
            DataObject.AddCopyingHandler(RichTextBox, CopyCommand);

            RichTextBox.MouseDoubleClick += (s, e) => ShowDetails();
            RichTextBox.PreviewMouseDown += (s, e) => CloseDetails();

            RichTextBox.KeyUp += (s, e) =>
            {
                EvaluateShowPopup(e);
                OnTextBoxKeyUp(e);
            };
            OnSegmentsChanged();
        }

        private void PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Cut)
            {
                Copy();
                RichTextBox.Selection.Text = string.Empty;
                e.Handled = true;
            }
        }

        private void PastCommand(object sender, DataObjectEventArgs e)
        {
            Paste();
            e.CancelCommand();
        }

        private void CopyCommand(object sender, DataObjectEventArgs e)
        {
            Copy();
            e.CancelCommand();
        }



        public override void OnApplyTemplate()
        {
            var window = Window.GetWindow(this);
            window.Deactivated += (s, e) => OnDeactivated();
            base.OnApplyTemplate();
        }

        private void OnDeactivated()
        {
            IntellisensePopup.Popup.IsOpen = false;
            CloseDetails();
        }

        private void CloseDetails()
        {
            if (!(GetFirstItemFromSelection() is { } item))
                return;

            item.CloseDetails();
        }

        private void ShowDetails()
        {
            if (!(GetFirstItemFromSelection() is { } item))
                return;

            var rect = RichTextBox.CaretPosition.GetCharacterRect(LogicalDirection.Backward);
            item.ShowDetails(rect);
        }

        private IntellisenseItem GetFirstItemFromSelection()
        {
            if (!(RichTextBox.Selection?.Start?.Parent is Run run && run.NextInline is InlineUIContainer container &&
                  container.Child is IntellisenseItem item))
                return null;

            return item;
        }

        private void GroupStyleChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IntellisensePopup.UpdateGroupStyle(GroupStyle);
        }
    }
}
