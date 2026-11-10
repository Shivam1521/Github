using Azure.Core;
using Dapper;
using Entities;
using IRepository;
using Microsoft.Data.SqlClient;
using Repository.Data;
using System.Data;

namespace Repository
{
    public class UserRepository(DapperContext context) : IUserRepository
    {
        //public async Task<UserMaster?> Authenticate(string username, string password)
        //{
        //    const string sql = @"IF EXISTS (SELECT TOP 1 * FROM UserMaster WHERE (Email = @Username OR MobileNo = @Username) and IsActive = 1) SELECT TOP 1 * FROM UserMaster WHERE (Email = @Username OR MobileNo = @Username) and IsActive = 1 ELSE SELECT 0 as UserId";

        //    using var connection = context.CreateConnection();
        //    var user = await connection.QueryFirstAsync<UserMaster>(sql, new { Username = username });

        //    if (user == null || user.UserId == 0 || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        //        return null;

        //    var a = BCrypt.Net.BCrypt.InterrogateHash(user.PasswordHash);
        //    return user; 
        //}

        public async Task<UserMaster?> Authenticate(string username, string password)
        {
            const string sql = @"
        SELECT TOP 1 *
        FROM UserMaster
        WHERE (Email = @Username OR MobileNo = @Username)
          AND IsActive = 1";

            using var connection = context.CreateConnection();

            var user = await connection.QueryFirstOrDefaultAsync<UserMaster>(
                sql,
                new { Username = username }
            );

            if (user == null)
                return null;

            // Already blocked
            if (user.IsBlocked)
                return user;

            // Wrong password
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                const string updateSql = @"
            UPDATE UserMaster
            SET FailedLoginAttempts = FailedLoginAttempts + 1,
                IsBlocked = CASE 
                                WHEN FailedLoginAttempts + 1 >= 5 THEN 1 
                                ELSE IsBlocked 
                            END
            WHERE UserId = @UserId";

                await connection.ExecuteAsync(updateSql, new { user.UserId });

                return null;
            }

            // Successful login - reset failed attempts
            const string resetSql = @"
        UPDATE UserMaster
        SET FailedLoginAttempts = 0
        WHERE UserId = @UserId";

            await connection.ExecuteAsync(resetSql, new { user.UserId });

            return user;
        }

        public async Task<IEnumerable<UserMasterReponse>> GetAllUsersAsync(TableRequest request)
        {
            using var connection = context.CreateConnection();
            var users = await connection.QueryAsync<UserMasterReponse>("GetUsers", new
            {
                request.PageNo,
                request.PageSize,
                request.SearchValue,
                request.SortColumn,
                request.SortOrder,
                request.UnitId
            }, commandType: CommandType.StoredProcedure);
            return users;
        }

        public async Task<Response> CreateUpdateUser(UserMasterRequest request)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryFirstAsync<Response>("CreateUpdateUser", new
            {
                request.UserId,
                request.UserCode,
                request.UserName,
                request.RoleId,
                request.UserType,
                request.Email,
                request.MobileNo,
                request.PasswordHash,
                request.UnitId,
                request.WHZoneId,
                Lines = request.Lines.AsTableValuedParameter("dbo.Lines", ["LineId"]),
                request.IsResetPassword,
                request.IsActive,
                request.CreatedBy
            }, commandType: CommandType.StoredProcedure);
            return result;
        }


        public async Task<Response> CreateEndUser(MobileUser request)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryFirstAsync<Response>("CreateMobileUser", new
            {

                request.UserId,
                request.UserName,
                request.UserType,
                request.Email,
                request.MobileNo,
                request.PasswordHash,
               
              
                request.IsActive,
               
            }, commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<Response> UpdateMPIN(MPINRequest request)
        {
            using var connection = context.CreateConnection();
            var users = await connection.QueryFirstAsync<Response>("UpdateMPIN", new
            {
                request.UserId,
                request.MPIN
            }, commandType: CommandType.StoredProcedure);
            return users;
        }

        public async Task<UserMaster?> GetUserDetails(int userId)
        {
            const string sql = @"IF EXISTS (SELECT TOP 1 * FROM UserMaster WHERE UserId = @UserId) SELECT TOP 1 * FROM UserMaster WHERE UserId = @UserId ELSE SELECT 0 as UserId";
            using var connection = context.CreateConnection();
            var user = await connection.QueryFirstAsync<UserMaster>(sql, new { UserId = userId });
            return user;
        }

        public async Task<Response> UpdatePasswordAsync(int userId, string PasswordHash)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryFirstAsync<Response>("UpdateUserPassword", new
            {
                userId,
                PasswordHash
            }, commandType: CommandType.StoredProcedure);
            return result;
        }
 
        public async Task<Response> DeleteUserWeb(string Email)
        {
            using var connection = context.CreateConnection();
            {
                var _resResult = await connection.QueryFirstAsync<Response>("DeleteMobileUserByWeb", new
                {
                    @Email = Email
                }, commandType: CommandType.StoredProcedure);

                return _resResult;
            }
        }

        public async Task<Response> DeleteUserMobile(int UserId)
        {
            using var connection = context.CreateConnection();
            {
                var _resResult = await connection.QueryFirstAsync<Response>("DeleteMobileUser", new
                {
                    @UserId = UserId
                }, commandType: CommandType.StoredProcedure);

                return _resResult;
            }
        }

    }
}
