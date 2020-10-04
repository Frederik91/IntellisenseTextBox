using GalaSoft.MvvmLight;

namespace SmartTestBox.Demo
{
    public class ItemViewModel : ViewModelBase
    {
        public string DisplayText { get; set; }

        public ItemViewModel(string displayText)
        {
            DisplayText = displayText;
        }   
    }
}