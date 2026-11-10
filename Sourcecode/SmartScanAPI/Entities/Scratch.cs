namespace Entities
{
    public class Scratch
    {
        public long SeqNo { get; set; }
        public required string ScratchCode { get; set; }
    }

    public class ScratchDetails
    {
        public required int Id { get; set; }
        public required string SONo { get; set; }
        public required long FromNo { get; set; }
        public required long ToNo { get; set; }
        public required long Qty { get; set; }
        public required int IssuedBy { get; set; }
        public DateTime IssuedOn { get; set; }
        public int TotalRows { get; set; }
    }

    public class ScratchRequest
    {
        public required string SONo { get; set; }
        public required int Qty { get; set; }
        public int IssuedBy { get; set; }
    }
}
