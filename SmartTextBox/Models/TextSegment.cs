namespace SmartTextBox.Models
{
    public class TextSegment : SegmentBase
    {
        public string Text { get; set; }

        protected bool Equals(TextSegment other)
        {
            return Text == other.Text;
        }

        public override int GetHashCode()
        {
            return (Text != null ? Text.GetHashCode() : 0);
        }
    }
}