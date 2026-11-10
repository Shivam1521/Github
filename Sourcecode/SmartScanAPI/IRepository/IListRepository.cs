using Entities;

namespace IRepository
{
    public interface IListRepository
    {
        Task<IEnumerable<DDL>> GetList(string listName, int? unitId);
        Task<IEnumerable<ListMaster>> GetListHeaders();
        Task<IEnumerable<ListDetailResponse>> GetListDetails(TableRequest request);
        Task<Response> AddUpdateListMaster(ListDetail detail);
    }
}
