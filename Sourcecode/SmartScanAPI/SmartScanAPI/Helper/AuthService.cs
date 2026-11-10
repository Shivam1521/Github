using Entities;
using IRepository;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace SmartScanAPI.Helper
{
    public class AuthService(IWrapperRepository repo, IOptions<JwtSettings> settings)
    {
        private readonly TokenGenerator _tokenGen = new(settings.Value);

        public async Task<AuthResponse?> Authenticate(AuthRequest request)
        {
            var user = await repo.User.Authenticate(request.Username, request.Password);
            if (user == null)
                return null;

            var token = _tokenGen.GenerateToken(user.UserId, user.Email, user.RoleId, user.TokenVersion);

            var refreshToken = _tokenGen.CreateRefreshToken(user.UserId);

            await repo.RefreshToken.SaveAsync(refreshToken);

            var _pagepermissions = await repo.Role.GetPages(user.RoleId);

            if (_pagepermissions != null)
            {
                foreach (var group in _pagepermissions)
                {
                    group.Children = JsonConvert.DeserializeObject<IEnumerable<AssignedPages>>(group.ChildrenJSON);
                }
            }

            return new AuthResponse
            {
                UserId = user.UserId,
                UserName = user.UserName,
                RoleId = user.RoleId,
                UnitId = user.UnitId,
                UserType = user.UserType,
                Email = user.Email,
                MobileNo = user.MobileNo,
                IsResetPassword = user.IsResetPassword,
                AssignedPermissionsForUser = _pagepermissions,
                Token = token,
                RefreshToken = refreshToken.Token,
                IsSA = user.RoleId == 1
            };
        }

        public async Task<AuthMobileResponse?> AuthenticateMobile(AuthRequest request)
        {
            var user = await repo.User.Authenticate(request.Username, request.Password);
            if (user == null)
                return null;

            if (user.IsBlocked)
                {
                    return new AuthMobileResponse
                    {
                        UserId = user.UserId,
                        UserName = user.UserName,
                        RoleId = user.RoleId,
                        UserType = user.UserType,
                        Email = user.Email,
                        MobileNo = user.MobileNo,
                        Token = string.Empty,
                        RefreshToken = string.Empty,
                        IsResetPassword = user.IsResetPassword,
                        IsBlocked = true
                    };
                
            }

            var token = _tokenGen.GenerateToken(user.UserId, user.Email, user.RoleId, user.TokenVersion);

            if (user.UserType != 1)
            {
                return new AuthMobileResponse
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    RoleId = user.RoleId,
                    UserType = user.UserType,
                    Email = user.Email,
                    MobileNo = user.MobileNo,
                    Token = token,
                    RefreshToken = TokenGenerator.GenerateRefreshToken(),
                    IsResetPassword = user.IsResetPassword
                };
            }
            else
            {
                return null;
            }
        }

        public async Task<AuthenticatedResponse?> RefreshTokens(int userId, string refreshToken, string email, int roleId, int tokenVersion)
        {
            var tokenHash = TokenGenerator.HashToken(refreshToken);
            var storedToken = await repo.RefreshToken.GetAsync(userId, tokenHash);

            if (storedToken == null)
                return null;

            if (!CryptographicOperations.FixedTimeEquals(storedToken.TokenHash, tokenHash))
                return null;

            if (storedToken.ExpiresAt <= DateTime.UtcNow)
                return null;

            if ((DateTime.UtcNow - storedToken.LastUsedAt).TotalMinutes > 60)
                return null;

            storedToken.RevokedAt = DateTime.UtcNow;

            var newRefreshToken = _tokenGen.CreateRefreshToken(userId);
            storedToken.ReplacedByTokenHash = newRefreshToken.TokenHash;

            await repo.RefreshToken.UpdateAsync(storedToken);
            await repo.RefreshToken.SaveAsync(newRefreshToken);

            var accessToken = _tokenGen.GenerateToken(userId, email, roleId, tokenVersion);

            return new AuthenticatedResponse
            {
                Token = accessToken,
                RefreshToken = newRefreshToken.Token
            };
        }
    }
}
