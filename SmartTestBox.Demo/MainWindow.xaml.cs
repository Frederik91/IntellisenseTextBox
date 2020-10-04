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
            new ItemViewModel("Apple", "Fruit"),
            new ItemViewModel("Banana", "Fruit"),
            new ItemViewModel("Cake", "Desert")
        };

        private ICollectionView Suggestions { get; }

        public MainWindow()
        {
            InitializeComponent();
            Suggestions = new ListCollectionView(_suggestionsSource) { Filter = o => Filter((ItemViewModel)o) };
            Suggestions.GroupDescriptions.Add(new PropertyGroupDescription("Group"));
            IntellisenseTextBox.ItemsSource = Suggestions;
            IntellisenseTextBox.Segments = new List<SegmentBase> { new TextSegment { Text = "This " }, new ObjectSegment { Content = _suggestionsSource[0], }, new TextSegment { Text = " a " }, new ObjectSegment { Content = _suggestionsSource[1] }, new TextSegment { Text = " with" }, new ObjectSegment { Content = _suggestionsSource[2] } };
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
