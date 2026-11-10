namespace Entities
{
    public class Line
    {
        public int LineId { get; set; }
    }

    public class LineDetails : Line
    {
        public string LineName { get; set; } = string.Empty;
    }
}
