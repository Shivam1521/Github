using Dapper;
using Entities;
using IRepository;
using Repository.Data;
using System.Data;

namespace Repository
{
    public class QRAuthenticationRepository(DapperContext context) : IQRAuthenticationRepository
    {
        public async Task<ResponseWithData> VerifyQR(QRAuthenticationRequest request)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryFirstAsync<ResponseWithData>("VerifyQR", new
            {
                request.QRData,
                request.Prefix,
                request.SeqNo,
                request.VerificationMode,
                request.City,
                request.State,
                request.Country,
                request.Latitude,
                request.Longitude,
                request.QRType
            }, commandType: CommandType.StoredProcedure, commandTimeout: 300);
            return result;
        }

        public async Task<ResponseWithData> InsertVerificationLog(QRAuthenticationRequest request)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryFirstAsync<ResponseWithData>("InsertVerificationLog", new
            {
                request.QRData,
                request.Prefix,
                request.SeqNo,
                request.VerificationMode,
                request.City,
                request.State,
                request.Country,
                request.Latitude,
                request.Longitude,
                request.QRType
            }, commandType: CommandType.StoredProcedure, commandTimeout: 300);
            return result;
        }
    }
}
