using System.Text.Json.Serialization;

namespace Entities
{
    public class UserMaster
    {
        public int UserId { get; set; }
        public required string UserCode { get; set; }
        public required string UserName { get; set; }
        public required int RoleId { get; set; }
        public required int UserType { get; set; }
        public required string Email { get; set; }
        public required string MobileNo { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public required int UnitId { get; set; }
        public int? WHZoneId { get; set; }
        public List<Line> Lines { get; set; } = [];
        public bool IsMPINEnabled { get; set; }
        public string MPIN { get; set; } = string.Empty;
        [JsonIgnore]
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public bool IsResetPassword { get; set; }
        public bool IsActive { get; set; }
        public int TokenVersion { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public long UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int FailedLoginAttempts { get; set; }

        public bool IsBlocked { get; set; }

    }

    public class MobileUser
    {
        public int UserId { get; set; }
      public required string UserName { get; set; }
    
        public required int UserType { get; set; }
        public required string Email { get; set; }
        public required string MobileNo { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool IsActive { get; set; }
       
    }

    public class UserMasterReponse : UserMaster
    {
        public string RoleName { get; set; } = string.Empty;
        public string UserTypeName { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public string WHZoneName { get; set; } = string.Empty;
        [JsonIgnore]
        public string Line { get; set; } = string.Empty;
        public int TotalRows { get; set; }
    }

    public class UserMasterRequest : UserMaster
    {
        public string Password { get; set; } = string.Empty;
    }

    public class MPINRequest
    {
        public required int UserId { get; set; }
        public required string MPIN { get; set; }
    }
}
