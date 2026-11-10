using Entities;

namespace IRepository
{
    public interface ITrackingRepository
    {
        Task<IEnumerable<CartonTraceResponse>> GetCartonTrace(long seqNo, string prefix, string qrType);
    }
}
