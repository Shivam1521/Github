
using Entities;

namespace IRepository
{
    public interface IRefreshTokenRepository
    {
        Task SaveAsync(RefreshToken token);
        Task<RefreshToken?> GetAsync(int userId, byte[] tokenHash);
        Task UpdateAsync(RefreshToken token);
        Task RevokeAllAsync(int userId);
    }
}
