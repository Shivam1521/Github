using System.Text.Json.Serialization;

namespace Entities
{
    public class PageMaster
    {
        public int PageId { get; set; }
        public string PageName { get; set; } = string.Empty;
        public string ControllerName { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
        public int ParentId { get; set; }
        public int SerialNo { get; set; }
        public bool IsAddAvailable { get; set; }
        public bool IsEditAvailable { get; set; }
        public bool IsActive { get; set; }
    }

    public class PageAccess
    {
        public string PageName { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
        public int SerialNo { get; set; }
        public bool IsView { get; set; }
        public bool IsAdd { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public int RoleId { get; set; }
        public int PageId { get; set; }
    }

    public class AssignedGroups
    {
        public string Group { get; set; } = string.Empty;
        [JsonIgnore]
        public string ChildrenJSON { get; set; } = string.Empty;
        public IEnumerable<AssignedPages>? Children { get; set; }
        public string Path { get; set; } = string.Empty;
    }

    public class AssignedPages
    {
        public int PageId { get; set; }
        public string PageName { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public bool IsAddAvailable { get; set; }
        public bool IsEditAvailable { get; set; }
        public bool IsView { get; set; }
        public bool IsAdd { get; set; }
        public bool IsEdit { get; set; }
    }

    public class PagePermissionRequest
    {
        public int PageId { get; set; }
        public int RoleId { get; set; }
        public bool IsView { get; set; }
        public bool IsAdd { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }

}
