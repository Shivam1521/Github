using Dapper;
using Entities;
using IRepository;
using Repository.Data;
using System.Data;

namespace Repository
{
    public class RoleRepository(DapperContext context) : IRoleRepository
    {
        public async Task<IEnumerable<RoleMasterReponse>> GetAllRolesAsync(TableRequest request)
        {
            using var connection = context.CreateConnection();
            var roles = await connection.QueryAsync<RoleMasterReponse>("GetRoles", new
            {
                request.PageNo,
                request.PageSize,
                request.SearchValue,
                request.SortColumn,
                request.SortOrder,
                request.UnitId
            }, commandType: CommandType.StoredProcedure);
            return roles;
        }

        public async Task<Response> CreateUpdateRole(RoleMaster request)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryFirstAsync<Response>("CreateUpdateRole", new
            {
                request.UnitId,
                request.RoleId,
                request.RoleName,
                request.RoleDesc,
                request.UserTypeId,
                request.IsActive,
                request.CreatedBy
            }, commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<List<AssignedGroups>> GetPages(int roleId)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryAsync<AssignedGroups>("GetPages", new
            {
                roleId
            }, commandType: CommandType.StoredProcedure);
            return [.. result];
        }

        public async Task<Response> AssignPermissions(List<PagePermissionRequest> permissions)
        {
            var table = new DataTable();
            table.Columns.Add("PageId", typeof(int));
            table.Columns.Add("RoleId", typeof(int));
            table.Columns.Add("IsView", typeof(bool));
            table.Columns.Add("IsAdd", typeof(bool));
            table.Columns.Add("IsEdit", typeof(bool));
            table.Columns.Add("IsDelete", typeof(bool));
            table.Columns.Add("IsActive", typeof(bool));
            table.Columns.Add("CreatedBy", typeof(string));

            foreach (var item in permissions)
            {
                table.Rows.Add(item.PageId, item.RoleId, item.IsView, item.IsAdd, item.IsEdit, item.IsDelete, item.IsActive, item.CreatedBy);
            }

            using var connection = context.CreateConnection();
            var result = await connection.QueryFirstAsync<Response>("AssignPagePermissions", new
            {
                Permissions = table.AsTableValuedParameter("PagePermissionType")
            }, commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<IEnumerable<DDL>> GetRolesDDL(int userType)
        {
            using var connection = context.CreateConnection();
            var list = await connection.QueryAsync<DDL>("GetRolesDDL", new
            {
                userType
            }, commandType: CommandType.StoredProcedure);
            return list;
        }
    }
}
