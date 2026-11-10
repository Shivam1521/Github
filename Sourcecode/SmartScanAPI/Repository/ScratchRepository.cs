using Dapper;
using Entities;
using IRepository;
using Repository.Data;
using System.Data;

using System.Threading;
using Dapper;

namespace Repository
{
    public class ScratchRepository(DapperContext context) : IScratchRepository
    {
        public async Task<Response> InsertScratchDetails(ScratchRequest request)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryFirstAsync<Response>("InsertScratchDetails", new
            {
                request.SONo,
                request.Qty,
                request.IssuedBy,
            }, commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<IEnumerable<ScratchDetails>> GetScratchDetails(TableRequest request)
        {
            using var connection = context.CreateConnection();
            var units = await connection.QueryAsync<ScratchDetails>("GetScratchDetails", new
            {
                request.PageNo,
                request.PageSize,
                request.SearchValue,
                request.SortColumn,
                request.SortOrder,
                request.UserId
            }, commandType: CommandType.StoredProcedure);
            return units;
        }

        public async Task<ScratchDetails> GetSODetails(string SONo, CancellationToken cancellationToken)
        {
            using var connection = context.CreateConnection();

            var cmd = new CommandDefinition(
                "GetSODetails",
                new { SONo },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken,
                commandTimeout: 30);

            var result = await connection.QueryFirstAsync<ScratchDetails>(cmd);
            return result;
        }
    }
}
