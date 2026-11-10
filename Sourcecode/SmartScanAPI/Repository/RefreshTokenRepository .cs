using Dapper;
using Entities;
using IRepository;
using Repository.Data;

namespace Repository
{
    public class RefreshTokenRepository(DapperContext context) : IRefreshTokenRepository
    {
        public async Task SaveAsync(RefreshToken token)
        {
            using var connection = context.CreateConnection();
            const string sql = @"
                INSERT INTO RefreshTokens
                (UserId, TokenHash, CreatedAt, LastUsedAt, ExpiresAt)
                VALUES
                (@UserId, @TokenHash, @CreatedAt, @LastUsedAt, @ExpiresAt)";

            await connection.ExecuteAsync(sql, token);
        }

        public async Task<RefreshToken?> GetAsync(int userId, byte[] tokenHash)
        {
            using var connection = context.CreateConnection();
            const string sql = @"
                SELECT *
                FROM RefreshTokens
                WHERE UserId = @UserId AND TokenHash = @TokenHash ORDER BY CreatedAt DESC";

            return await connection.QuerySingleOrDefaultAsync<RefreshToken>(
                sql,
                new { UserId = userId, TokenHash = tokenHash }
            );
        }

        public async Task UpdateAsync(RefreshToken token)
        {
            using var connection = context.CreateConnection();
            const string sql = @"
                UPDATE RefreshTokens
                SET
                    LastUsedAt = @LastUsedAt,
                    RevokedAt = @RevokedAt,
                    ReplacedByTokenHash = @ReplacedByTokenHash
                WHERE Id = @Id";

            await connection.ExecuteAsync(sql, token);
        }

        public async Task RevokeAllAsync(int userId)
        {
            using var connection = context.CreateConnection();
            const string sql = @"
                UPDATE RefreshTokens
                SET RevokedAt = @Now
                WHERE UserId = @UserId
                  AND RevokedAt IS NULL";

            await connection.ExecuteAsync(sql, new
            {
                UserId = userId,
                Now = DateTime.UtcNow
            });
        }
    }
}
