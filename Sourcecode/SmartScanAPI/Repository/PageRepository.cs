using Dapper;
using Entities;
using IRepository;
using Repository.Data;
using System.Data;

namespace Repository
{
    public class PageRepository(DapperContext context) : IPageRepository
    {
        public async Task<IEnumerable<ChildPage>> GetPagePermissions(long roleId)
        {

            using var connection = context.CreateConnection();
            var _resResult = await connection.QueryAsync<ChildPage>("GetPagePermissions", new
            {
                RoleId = roleId
            }, commandType: CommandType.StoredProcedure);

            return _resResult;
        }
    }
}
