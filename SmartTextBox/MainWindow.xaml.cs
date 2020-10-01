using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SmartTextBox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly List<string> _options = new List<string>
        {
            "Apple",
            "Banana",
            "Cake",
        };

        public MainWindow()
        {
            InitializeComponent();
            IntellisenseTextBox.Text = "this ${{is}} a ${{test}}";
            IntellisenseTextBox.SearchFunction = Search;
        }

        public List<string> Search(string input)
        {
            return _options.Where(x => x.Contains(input, StringComparison.CurrentCultureIgnoreCase)).ToList();
        }
    }
}
