namespace Entities
{
    internal class Profile
    {
    }

    public class ChangePasswordRequest
    {
        public required int UserId { get; set; }
        public required string OldPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}
