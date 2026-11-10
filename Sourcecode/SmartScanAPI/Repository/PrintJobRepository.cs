using Dapper;
using Entities;
using IRepository;
using Repository.Data;
using System.Data;

namespace Repository
{
    internal class PrintJobRepository(DapperContext context) : IPrintJobRepository
    {
        public async Task<ResponseWithData> CreatePrintJob(PrintJobRequest job)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryFirstAsync<ResponseWithData>("CreatePrintJob", new
            {
                job.SrNo,
                job.UserId,
                job.Quantity,
            }, commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<PrintJobDetails> GetJobDetails(string jobNo = "", int jobId = 0)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryFirstAsync<PrintJobDetails>("GetJobDetails", new
            {
                jobNo,
                jobId
            }, commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<LastPrintedNumberDetails> GetLastPrintedNumber(int jobId, int userId)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryFirstAsync<LastPrintedNumberDetails>("GetLastPrintedNumber", new
            {
                jobId,
                userId
            }, commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<Response> GetQRPrintStatus(PrintJobStatusUpdateRequest request)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryFirstAsync<Response>("GetQRPrintStatus", new
            {
                request.JobId,
                request.SeqNo,
                request.Type
            }, commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<Response> UpdateQRPrintStatus(PrintJobStatusUpdateRequest request)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryFirstAsync<Response>("UpdateQRPrintStatus", new
            {
                request.JobId,
                request.SeqNo,
                request.Type,
                request.UserId,
                request.Weight,
                request.PrinterIP
            }, commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<PrintJobDetails> GetPendingPrintJobs(int userId)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryFirstAsync<PrintJobDetails>("GetPendingPrintJobs", new
            {
                userId
            }, commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<MasterReprintResponse> GetMasterDetailFromOuter(QRDetailsRequest request)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryAsync<MasterReprintResponse>("GetMasterDetailFromOuter", new
            {
                request.SeqNo,
                request.Prefix,
                request.QRType
            }, commandType: CommandType.StoredProcedure);
            return result.First();
        }

        public async Task<MasterReprintResponse> GetMasterDetailForReprint(QRDetailsRequest request)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryAsync<MasterReprintResponse>("GetMasterDetailForReprint", new
            {
                request.SeqNo,
                request.Prefix,
                request.QRType
            }, commandType: CommandType.StoredProcedure);
            return result.First();
        }

        public async Task<CartonDetailsForAttachDetach> GetCartonDetailsForAttachDetach(CartonDetailsForAttachDetachRequest request)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryAsync<CartonDetailsForAttachDetach>("GetCartonDetailsForAttachDetach", new
            {
                request.SeqNo,
                request.Prefix,
                request.QRType,
                request.ActionFor,
                request.ActionType
            }, commandType: CommandType.StoredProcedure);
            return result.First();
        }

        public async Task<IEnumerable<ProductionLineQRsResponse>> GeneratePLQRs(ProductionLineQRsRequest request)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryAsync<ProductionLineQRsResponse>("GeneratePLQRs", new
            {
                request.ItemCode,
                request.Quantity,
                request.UserId,
            }, commandType: CommandType.StoredProcedure);
            return result;
        }
    }
}
