using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SmartTextBox.EventArgs;
using SmartTextBox.Models;

namespace SmartTestBox.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly List<ItemViewModel> _suggestionsSource = new List<ItemViewModel>
        {
            new ItemViewModel("Apple"),
            new ItemViewModel("Banana"),
            new ItemViewModel("Cake")
        };

        private ICollectionView Suggestions { get; }

        public MainWindow()
        {
            InitializeComponent();
            Suggestions = new ListCollectionView(_suggestionsSource) { Filter = o => Filter((ItemViewModel)o) };
            IntellisenseTextBox.ItemsSource = Suggestions;
            IntellisenseTextBox.Segments = new List<SegmentBase> { new TextSegment { Text = "This " }, new ObjectSegment { Content = new ItemViewModel("is"), }, new TextSegment { Text = " a " }, new ObjectSegment { Content = new ItemViewModel("test") } };
            IntellisenseTextBox.SearchChanged += (s, e) => Search();
            IntellisenseTextBox.SegmentsChanged += (s, e) => SegmentsChanged(e);
        }

        private void SegmentsChanged(SegmentsChangedEventArgs args)
        {
            SegmentCountTextBlock.Text = $"Segment count: {args.Segments.Count}";
        }

        private bool Filter(ItemViewModel item)
        {
            if (string.IsNullOrEmpty(IntellisenseTextBox.SearchText))
                return true;

            return item?.DisplayText?.Contains(IntellisenseTextBox.SearchText, StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        private void Search()
        {
            Suggestions.Refresh();
        }
    }
}
