using Entities;

namespace IRepository
{
    public interface IQCRepository
    {
        Task<CartonDetails> GetCartonDetails(string prefix, long seqNo, string qrType, int actionType);
        Task<Response> ReplaceOuterForQC(ReplaceCartonRequest request);
        Task<QCStatusDetails> GetQCStatus(string prefix, long seqNo, string qrType);
        Task<Response> MarkQCOpen(MarkQCOpenRequest request);
        Task<JobDetailsForQC> GetJobDetailsForQC(string prefix, long seqNo, string qrType);
        Task<Response> UpdateQCStatus(UpdateQCStatusRequest request);
        Task<CartonDetailsForQC> GetCartonDetailsForQC(string prefix, long seqNo, string qrType, int actionType);
        Task<Response> ReplaceOuterAfterQC(ReplaceOuterAfterQCRequest request);
        Task<Response> AttachDetachCartons(ReplaceCartonRequest request);
    }
}
