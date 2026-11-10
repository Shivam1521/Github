using Entities;

namespace IRepository
{
    public interface IPageRepository
    {
        Task<IEnumerable<ChildPage>> GetPagePermissions(long roleId);
    }
}
