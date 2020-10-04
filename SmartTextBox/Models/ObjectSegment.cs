namespace SmartTextBox.Models
{
    public class ObjectSegment : SegmentBase
    {
        public object Content { get; set; }

        protected bool Equals(ObjectSegment other)
        {
            return Equals(Content, other.Content);
        }

        public override int GetHashCode()
        {
            return (Content != null ? Content.GetHashCode() : 0);
        }
    }
}