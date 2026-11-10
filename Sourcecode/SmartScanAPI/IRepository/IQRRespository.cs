using Entities;

namespace IRepository
{
    public interface IQRRespository
    {
        Task<IEnumerable<OuterQRs>> GetOuterQRs(string prefix, long seqNo, string qrType);

        Task<IEnumerable<OuterDataQRs>> GetAllOuterQRs(string prefix, long seqNo, string qrType);

        Task<IEnumerable<OuterDataQRs>> GetOuterDetailsforReprint(string prefix, long seqNo, string qrType);
        Task<GetQRDetailsResponse> GetQRInfo(string prefix, long seqNo, string qrType);
    }
}
