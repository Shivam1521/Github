using System.Text.Json.Serialization;

namespace Entities
{
    public class AuthResponse
    {
        public int UserId { get; set; }
        public required string UserName { get; set; }
        public int RoleId { get; set; }
        public int UnitId { get; set; }
        public int UserType { get; set; }
        public required string Email { get; set; }
        public required string MobileNo { get; set; }
        public required string Token { get; set; }
        public required string RefreshToken { get; set; }
        public bool IsResetPassword { get; set; }
        public bool IsSA { get; set; }

        List<AssignedGroups>? _assignedPermissionsForUser = [];
        public List<AssignedGroups>? AssignedPermissionsForUser
        {
            get { return _assignedPermissionsForUser; }
            set { _assignedPermissionsForUser = value; }
        }
    }

    public class AuthMobileResponse
    {
        public int UserId { get; set; }
        public required string UserName { get; set; }
        public int RoleId { get; set; }
        public int UserType { get; set; }
        public required string Email { get; set; }
        public required string MobileNo { get; set; }
        public required string Token { get; set; }
        public required string RefreshToken { get; set; }
        public bool IsResetPassword { get; set; }
        public bool IsBlocked { get; set; }
    }

    public class RefreshToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public byte[] TokenHash { get; set; } = [];
        public DateTime CreatedAt { get; set; }
        public DateTime LastUsedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public byte[]? ReplacedByTokenHash { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsRevoked => RevokedAt.HasValue;
        public bool IsActive => !IsExpired && !IsRevoked;
    }

    public class ReCaptchaResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("score")]
        public decimal? Score { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("error-codes")]
        public List<string> ErrorCodes { get; set; }
    }

}
