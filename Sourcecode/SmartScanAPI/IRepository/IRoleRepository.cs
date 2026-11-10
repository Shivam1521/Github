using Entities;

namespace IRepository
{
    public interface IRoleRepository
    {
        Task<IEnumerable<RoleMasterReponse>> GetAllRolesAsync(TableRequest request);
        Task<Response> CreateUpdateRole(RoleMaster request);
        Task<List<AssignedGroups>> GetPages(int roleId);
        Task<Response> AssignPermissions(List<PagePermissionRequest> permissions);
        Task<IEnumerable<DDL>> GetRolesDDL(int userType);
    }
}
