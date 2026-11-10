namespace Entities
{
    public class ChildPage
    {
        public int PageId { get; set; }
        public string PageName { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
        public string ControllerName { get; set; } = string.Empty;
        public int ParentId { get; set; }
        public int RoleId { get; set; }
        public int SerialNo { get; set; }
        public bool IsActive { get; set; }
        public bool IsView { get; set; }
        public bool IsAdd { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsReport { get; set; }
    }
}
