using Dapper;
using Entities;
using IRepository;
using Repository.Data;
using System.Data;

namespace Repository
{
    public class ReportRepository(DapperContext context) : IReportRepository
    {

        public async Task<IEnumerable<SerialReport>> GetSerialUploadReport(TableRequest request)
        {
            using var connection = context.CreateConnection();

            var printingDetails = await connection.QueryAsync<SerialReport>("GetSerialUploadReport", new
            {
                request.PageNo,
                request.PageSize,
                request.SearchValue,
                request.SortColumn,
                request.SortOrder,
                request.UnitId,
                request.Type,
                request.FromDate,
                request.ToDate
            }, commandType: CommandType.StoredProcedure);

            return printingDetails;
        }

        public async Task<IEnumerable<PrintJobReport>> GetPrintJobReport(string Srno)
        {
            using var connection = context.CreateConnection();

            var query = "SELECT * FROM PrintJobs WHERE [SrNo] = @SrNo";

            var printingDetails = await connection.QueryAsync<PrintJobReport>(query, new { SrNo = Srno });

            return printingDetails;
        }
    }
}
