namespace Entities
{
    public class ListMaster
    {
        public required int ListId { get; set; }
        public required string ListName { get; set; }
    }

    public class ListDetail
    {
        public required int ListDetailId { get; set; }
        public required int ListMasterId { get; set; }
        public required string ListDetailName { get; set; }
        public required bool IsActive { get; set; }
        public int? UnitId { get; set; }
        public int UserId { get; set; }
    }

    public class ListRequest
    {
        public required int ListId { get; set; }
        public required string ListDetailName { get; set; }
    }

    public class ListDetailResponse : ListDetail
    {
        public string ListName { get; set; } = string.Empty;
        public string CreatedDate { get; set; } = string.Empty;
        public int TotalRows { get; set; }
    }
}
