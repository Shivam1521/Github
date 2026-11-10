using Entities;

namespace IRepository
{
    public interface IPreprintedQRJobRepository
    {
        Task<Response> CreatePreprintedQRJob(PreprintedQRJobRequest request);
        Task<Response> CreatePreprintedQRJob_supplier(PreprintedQRJobSupplierRequest request);
        Task<MappingResponse> CreatePreprintedQRMapping(PreprintedQRMappingDBRequest request);
        Task<IEnumerable<PreprintedQRDataResponse>> GetPreprintedQRJobs(PreprintedQRDataRequest request);

        Task<IEnumerable<PreprintedsupplierQRDataResponse>> GetPreprintedQRJobssupplier(PreprintedsupplierQRDataRequest request);
        Task<PreprintedQRDataResponse> GetPreprintedQRJobDetails(int Id);
        Task<PreprintedQRDataResponse> GetPreprintedQRJobDetails_s(int Id);

        Task<IEnumerable<PreprintedsupplierQRDataResponse>> GetPreprintedQRJobDetailsSupplier(int Id);
        Task<Response> UpdatePreprintedQRJobStatus(long id, int userId);

        Task<Response> UpdatePreprintedQRJobStatussupplier(long id, int userId);
        Task<List<PrintedSeq>> GetPrintedSeq();
        Task<PreprintedQRReprintResponse> GetPreprintedQrDetailsforReprint(PreprintedQRReprintRequest request);

        Task<IEnumerable<PendingPushRecord>> GetPendingPreprintedQRForPush();

        Task<int> UpdatePreprintedQRPushAttempt(string ids);

        Task<int> UpdatePreprintedQRPushResponse(string ids, string response);
    }
}
