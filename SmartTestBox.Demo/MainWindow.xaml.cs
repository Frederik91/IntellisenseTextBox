using System;
using System.Collections.Generic;
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
using SmartTestBox.Demo.Models;
using SmartTextBox.Models;

namespace SmartTestBox.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly List<Suggestion> _suggestions = new List<Suggestion>
        {
            new Suggestion {DisplayText = "Apple", Content = new ItemViewModel("Apple") },
            new Suggestion {DisplayText = "Banana", Content = new ItemViewModel("Banana")},
            new Suggestion {DisplayText = "Cake", Content = new ItemViewModel("Cake")}
        };

        public MainWindow()
        {
            InitializeComponent();
            IntellisenseTextBox.Segments = new List<SegmentBase> { new TextSegment { Text = "This " }, new ObjectSegment { Content = new ItemViewModel("is"), }, new TextSegment { Text = " a " }, new ObjectSegment { Content = new ItemViewModel("test"), } };
            IntellisenseTextBox.SearchFunction = Search;
        }

        private List<object> Search(string input)
        {
            return _suggestions.Where(x => x.DisplayText.Contains(input, StringComparison.CurrentCultureIgnoreCase)).OfType<object>().ToList();
        }
    }
}
