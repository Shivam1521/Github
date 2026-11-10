namespace Entities
{
    public class RoleMaster
    {
        public int? UnitId { get; set; }
        public int RoleId { get; set; }
        public int UserTypeId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string RoleDesc { get; set; } = string.Empty;
        public string UserType { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

    public class RoleMasterReponse : RoleMaster
    {
        public string UnitName { get; set; } = string.Empty;
        public int TotalRows { get; set; }
    }
}
