using Entities;
using System.Data;

namespace IRepository
{
    public interface IItemRepository
    {
        Task<IEnumerable<ItemDDL>> GetItems(int userId);
        Task<IEnumerable<ItemDetailsResponse>> GetItemDetails(int id, int userId);
        Task<IEnumerable<ItemMasterResponse>> GetItemMasterByUserId(TableRequest request);
        Task<Response> UpdateItemMaster(ItemMasterRequest request, int createdby);
        Task<Response> MapItemsAndUsers(DataTable tvp);
    }
}
