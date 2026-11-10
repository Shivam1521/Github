using Dapper;
using Entities;
using IRepository;
using Repository.Data;
using System.Data;

namespace Repository
{
    public class QRRespository(DapperContext context) : IQRRespository
    {
        public async Task<IEnumerable<OuterQRs>> GetOuterQRs(string prefix, long seqNo, string qrType)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryAsync<OuterQRs>("GetOuterQRs", new
            {
                prefix,
                seqNo,
                qrType
            }, commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<IEnumerable<OuterDataQRs>> GetAllOuterQRs(string prefix, long seqNo, string qrType)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryAsync<OuterDataQRs>("GetOuterQRs", new
            {
                prefix,
                seqNo,
                qrType
            }, commandType: CommandType.StoredProcedure);
            return result;
        }
         public async Task<IEnumerable<OuterDataQRs>> GetOuterDetailsforReprint(string prefix, long seqNo, string qrType)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryAsync<OuterDataQRs>("GetOuterDetailsforReprint", new
            {
                prefix,
                seqNo,
                qrType
            }, commandType: CommandType.StoredProcedure);
            return result;
        }
        

        public async Task<GetQRDetailsResponse> GetQRInfo(string prefix, long seqNo, string qrType)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryFirstAsync<GetQRDetailsResponse>("GetQRInfo", new
            {
                prefix,
                seqNo,
                qrType
            }, commandType: CommandType.StoredProcedure);
            return result;
        }
    }
}
