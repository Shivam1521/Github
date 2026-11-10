namespace Entities
{
    internal class Tracking
    {
    }

    public class CartonTraceResponse
    {
        public string Activity { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string DateTimeStamp { get; set; } = string.Empty;
        public string Remark { get; set; } = string.Empty;
    }
}
