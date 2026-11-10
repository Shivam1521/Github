using Entities;

namespace IRepository
{
    public interface IQRAuthenticationRepository
    {
        Task<ResponseWithData> VerifyQR(QRAuthenticationRequest request);
        Task<ResponseWithData> InsertVerificationLog(QRAuthenticationRequest request);
    }
}
