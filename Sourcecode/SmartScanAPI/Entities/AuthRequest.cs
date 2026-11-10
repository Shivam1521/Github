namespace Entities
{
    public class AuthRequest
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public string CaptchaToken { get; set; } = string.Empty;
    }

    public class RefreshTokenRequest
    {
        public required string RefreshToken { get; set; }
        public required string AccessToken { get; set; }
    }

    public class AuthenticatedResponse
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
    public class LoginAttemptResponse
    {
        public int ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public int UserId { get; set; }
        public int FailedLoginAttempts { get; set; }
        public bool IsBlocked { get; set; }
        public string LoginStatus { get; set; }
    }
}
