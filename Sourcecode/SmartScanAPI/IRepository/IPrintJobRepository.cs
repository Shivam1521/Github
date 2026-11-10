using Entities;

namespace IRepository
{
    public interface IPrintJobRepository
    {
        Task<ResponseWithData> CreatePrintJob(PrintJobRequest job);
        Task<PrintJobDetails> GetJobDetails(string jobNo = "", int jobId = 0);
        Task<LastPrintedNumberDetails> GetLastPrintedNumber(int jobId, int userId);
        Task<Response> UpdateQRPrintStatus(PrintJobStatusUpdateRequest request);
        Task<MasterReprintResponse> GetMasterDetailFromOuter(QRDetailsRequest request);
        Task<MasterReprintResponse> GetMasterDetailForReprint(QRDetailsRequest request);
        Task<CartonDetailsForAttachDetach> GetCartonDetailsForAttachDetach(CartonDetailsForAttachDetachRequest request);
        Task<Response> GetQRPrintStatus(PrintJobStatusUpdateRequest request);
        Task<IEnumerable<ProductionLineQRsResponse>> GeneratePLQRs(ProductionLineQRsRequest request);
    }
}
