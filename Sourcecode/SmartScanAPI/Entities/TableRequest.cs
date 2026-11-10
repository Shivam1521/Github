namespace Entities
{
    public class TableRequest
    {
        public int PageNo { get; set; }
        public int? First { get; set; }
        public string SearchValue { get; set; } = string.Empty;
        public string SearchColumn { get; set; } = string.Empty;
        public int PageSize { get; set; }
        public string SortColumn { get; set; } = "CreatedDate";
        public string SortOrder { get; set; } = "DESC";
        public int UserId { get; set; }
        public string? UnitId { get; set; }
        public string FromDate { get; set; } = string.Empty;
        public string ToDate { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}
