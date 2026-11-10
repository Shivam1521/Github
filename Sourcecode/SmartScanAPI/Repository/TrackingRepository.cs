using Dapper;
using Entities;
using IRepository;
using Repository.Data;
using System.Data;

namespace Repository
{
    public class TrackingRepository(DapperContext context) : ITrackingRepository
    {
        public async Task<IEnumerable<CartonTraceResponse>> GetCartonTrace(long seqNo, string prefix, string qrType)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryAsync<CartonTraceResponse>("GetCartonTrace", new
            {
                prefix,
                seqNo,
                qrType
            }, commandType: CommandType.StoredProcedure);
            return result;
        }
    }
}
