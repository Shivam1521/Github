using Entities;

namespace IRepository
{
    public interface IERPRepository
    {
        Task<Response> MergeSKUs(IEnumerable<Items> request, int userId);
        Task<Response> MergePlants(IEnumerable<Plants> SKUs, int userId);
        Task<Response> InsertPickSlips(IEnumerable<PickSlips> pickSlips, int userId);
        Task<Response> MergeInsertPickSlips(IEnumerable<MergePickSlips> pickSlips, int userId);
        Task<Response> InsertUpdateKanban(IEnumerable<Kanban> list, int userId);
        Task<Response> LogEBSAPIRequestResponse(EBSAPILog log);
    }
}
