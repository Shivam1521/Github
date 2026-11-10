using Entities;

namespace IRepository
{
    public interface IScratchRepository
    {
        Task<Response> InsertScratchDetails(ScratchRequest request);
        Task<IEnumerable<ScratchDetails>> GetScratchDetails(TableRequest request);
        Task<ScratchDetails> GetSODetails(string SONo, CancellationToken cancellationToken);
    }
}
