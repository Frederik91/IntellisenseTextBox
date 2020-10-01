using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Command;

namespace SmartTextBox
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:AwesomeTextBox"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:AwesomeTextBox;assembly=AwesomeTextBox"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:AutocompleteTextBox/>
    ///
    /// </summary>
    ///
    [ContentProperty("Text")]
    public class IntellisenseTextBox : Control
    {
        public static readonly DependencyProperty SearchFunctionProperty = DependencyProperty.Register(
            "SearchFunction", typeof(Func<string, List<string>>), typeof(IntellisenseTextBox), new PropertyMetadata(default(Func<string, List<string>>)));

        public Func<string, List<string>> SearchFunction
        {
            get { return (Func<string, List<string>>)GetValue(SearchFunctionProperty); }
            set { SetValue(SearchFunctionProperty, value); }
        }


        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(IntellisenseTextBox), new PropertyMetadata(default(string)));

        public RichTextBox RichTextBox { get; private set; }

        public static readonly DependencyProperty IsPopupOpenProperty = DependencyProperty.Register(
            "IsPopupOpen", typeof(bool), typeof(IntellisenseTextBox), new PropertyMetadata(default(bool)));

        public bool IsPopupOpen
        {
            get { return (bool)GetValue(IsPopupOpenProperty); }
            set { SetValue(IsPopupOpenProperty, value); }
        }

        public static readonly DependencyProperty PopupPlacementTargetProperty = DependencyProperty.Register(
            "PopupPlacementTarget", typeof(UIElement), typeof(IntellisenseTextBox), new PropertyMetadata(default(UIElement)));

        public UIElement PopupPlacementTarget
        {
            get { return (UIElement)GetValue(PopupPlacementTargetProperty); }
            set { SetValue(PopupPlacementTargetProperty, value); }
        }

        public static readonly DependencyProperty PopupPlacementRectangleProperty = DependencyProperty.Register(
            "PopupPlacementRectangle", typeof(Rect), typeof(IntellisenseTextBox), new PropertyMetadata(default(Rect)));

        public Rect PopupPlacementRectangle
        {
            get { return (Rect)GetValue(PopupPlacementRectangleProperty); }
            set { SetValue(PopupPlacementRectangleProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty OptionsProperty = DependencyProperty.Register(
            "Options", typeof(List<string>), typeof(IntellisenseTextBox), new PropertyMetadata(default(List<string>)));

        public List<string> Options
        {
            get { return (List<string>)GetValue(OptionsProperty); }
            set { SetValue(OptionsProperty, value); }
        }

        public ICommand TextChangedCommand { get; }

        public ICommand LoadedCommand { get; }

        public IntellisenseTextBox()
        {
            TextChangedCommand = new RelayCommand<TextChangedEventArgs>(TextChanged);
            LoadedCommand = new RelayCommand<RichTextBox>(OnLoaded);
        }


        private void OnLoaded(RichTextBox richTextBox)
        {
            if (RichTextBox != null)
                return;

            RichTextBox = richTextBox;

            if (!(RichTextBox.Document.Blocks.FirstBlock is Paragraph paragraph))
                return;

            var parts = new List<string>();
            var remainingText = Text;
            while (!string.IsNullOrEmpty(remainingText))
            {
                var firstVariable = PathVariableUtils.VariableRegex.Match(remainingText).Value;
                if (string.IsNullOrEmpty(firstVariable))
                {
                    parts.Add(remainingText);
                    return;
                }
                if (remainingText.StartsWith(firstVariable))
                {
                    parts.Add(firstVariable);
                    remainingText = remainingText.Remove(0, firstVariable.Length);
                    continue;
                }

                var text = remainingText.Split(firstVariable)[0];
                parts.Add(text);
                remainingText = remainingText.Remove(0, text.Length);
            }

            foreach (var part in parts)
            {
                if (!PathVariableUtils.ContainsVariable(part))
                {
                    paragraph.Inlines.Add(part);
                    continue;
                }
                paragraph.Inlines.Add(new Button { Content = PathVariableUtils.VariableNameRegex.Match(part).Value });


            }
        }


        private void TextChanged(TextChangedEventArgs obj)
        {
            if (RichTextBox is null)
                return;

            if (!(RichTextBox?.Document.Blocks.FirstBlock is Paragraph para))
                return;

            if (ParagraphContainsVariables(para))
                ConvertParagraphs(para);

            var characterAtCarrot = RichTextBox.CaretPosition.GetPointerContext(LogicalDirection.Backward);
            if (characterAtCarrot != TextPointerContext.Text)
                return;

            var start = RichTextBox.CaretPosition;  // this is the variable we will advance to the left until a non-letter character is found

            var stringBeforeCaret = start.GetTextInRun(LogicalDirection.Backward);   // extract the text in the current run from the caret to the left

            if (stringBeforeCaret.Contains($"${{"))
            {
                var searchText = GetSearchText(stringBeforeCaret);
                Options = SearchFunction?.Invoke(searchText);
            }

            if (!(stringBeforeCaret.EndsWith("$") || stringBeforeCaret.EndsWith("${") ||
                  stringBeforeCaret.EndsWith("${{")))
            {
                if (IsPopupOpen && !stringBeforeCaret.Contains("${{"))
                    IsPopupOpen = false;
                return;
            }


            ShowPopup();
        }

        private string GetSearchText(string stringBeforeCaret)
        {
            if (stringBeforeCaret.EndsWith("$") || stringBeforeCaret.EndsWith("${") || stringBeforeCaret.EndsWith("${{"))
                return string.Empty;

            return stringBeforeCaret.Remove(0, stringBeforeCaret.IndexOf("${{", StringComparison.InvariantCulture) + 3).TrimStart();
        }

        private void ShowPopup()
        {
            var rect = RichTextBox.CaretPosition.GetCharacterRect(LogicalDirection.Backward);
            PopupPlacementTarget = RichTextBox;
            PopupPlacementRectangle = rect;
            IsPopupOpen = true;
            Options = SearchFunction?.Invoke(string.Empty);
        }

        private void ConvertParagraphs(Paragraph para)
        {
            while (ParagraphContainsVariables(para))
            {
                var inline = para.Inlines.OfType<Run>().First(x => PathVariableUtils.ContainsVariable(x.Text));
                var variable = PathVariableUtils.VariableRegex.Match(inline.Text).Value;
                var split = inline.Text.Split(variable, 2);
                inline.Text = split[0];
                var newInline = new InlineUIContainer(new Button
                { Content = PathVariableUtils.VariableNameRegex.Match(variable).Value });
                para.Inlines.InsertAfter(inline, newInline);
                if (split.Length > 0)
                    para.Inlines.Add(PathVariableUtils.VariableRegex.Split(split[1])[0]);
                RichTextBox.CaretPosition = RichTextBox.CaretPosition.DocumentEnd;
            }
        }

        private static bool ParagraphContainsVariables(Paragraph para)
        {
            return para.Inlines.OfType<Run>().Any(x => PathVariableUtils.ContainsVariable(x.Text));
        }

        static IntellisenseTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IntellisenseTextBox), new FrameworkPropertyMetadata(typeof(IntellisenseTextBox)));
        }
    }
}
