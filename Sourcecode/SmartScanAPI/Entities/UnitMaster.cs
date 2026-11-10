namespace Entities
{
    public class UnitMaster
    {
        public int UnitId { get; set; }
        //public required int LocationId { get; set; }
        public required string UnitName { get; set; }
        public required string UnitCode { get; set; }
        public required string UnitType { get; set; }
        //public string Email { get; set; } = string.Empty;
        //public string MobileNumber { get; set; } = string.Empty;
        public required string Address1 { get; set; }
        public required string Address2 { get; set; }
        public required string Address3 { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string Country { get; set; }
        public required string Pincode { get; set; }
        public string Prefix { get; set; } = string.Empty;
    }

    public class UnitMasterRequest : UnitMaster
    {
        public int UserId { get; set; }
    }

    public class UnitMasterResponse : UnitMaster
    {
        public string LocationName { get; set; } = string.Empty;
        public int TotalRows { get; set; }
    }
}
