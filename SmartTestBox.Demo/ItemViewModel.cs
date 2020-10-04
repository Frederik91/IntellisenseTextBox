using GalaSoft.MvvmLight;

namespace SmartTestBox.Demo
{
    public class ItemViewModel : ViewModelBase
    {
        public string DisplayText { get; set; }
        public string Group { get; }

        public ItemViewModel(string displayText, string group)
        {
            DisplayText = displayText;
            Group = @group;
        }   
    }
}