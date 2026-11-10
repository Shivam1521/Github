using Entities;

namespace IRepository
{
    public interface IUserRepository
    {
        Task<UserMaster?> Authenticate(string username, string password);
        Task<IEnumerable<UserMasterReponse>> GetAllUsersAsync(TableRequest request);
        Task<Response> CreateUpdateUser(UserMasterRequest request);

        Task<Response> CreateEndUser(MobileUser request);
        Task<Response> UpdateMPIN(MPINRequest request);
        Task<UserMaster?> GetUserDetails(int userId);
        Task<Response> UpdatePasswordAsync(int userId, string newPasswordHash);
        Task<Response> DeleteUserWeb(string Email);

        Task<Response> DeleteUserMobile(int UserId);
    }
}
