using Dapper;
using Entities;
using IRepository;
using Repository.Data;
using System.Data;

namespace Repository
{
    public class PreprintedQRJobRepository(DapperContext context) : IPreprintedQRJobRepository
    {
        public async Task<IEnumerable<PreprintedQRDataResponse>> GetPreprintedQRJobs(PreprintedQRDataRequest request)
        {
            using var connection = context.CreateConnection();
            var list = await connection.QueryAsync<PreprintedQRDataResponse>("GetPreprintedQRJobs", new
            {
                request.UserId,
                request.QRType,
                request.IsExistingStock
            }, commandType: CommandType.StoredProcedure);
            return list;
        }

        public async Task<IEnumerable<PreprintedsupplierQRDataResponse>> GetPreprintedQRJobssupplier(PreprintedsupplierQRDataRequest request)
        {
            using var connection = context.CreateConnection();
            var list = await connection.QueryAsync<PreprintedsupplierQRDataResponse>("GetPreprintedQRJobssupplier", new
            {
                request.UserId,
                request.QRType
            }, commandType: CommandType.StoredProcedure);
            return list;
        }

        public async Task<Response> CreatePreprintedQRJob(PreprintedQRJobRequest request)
        {
            using var connection = context.CreateConnection();
            var list = await connection.QueryFirstAsync<Response>("CreatePreprintedQRJob", new
            {
                request.Qty,
                request.ItemCode,
                request.QRType,
                request.IsExistingStock,
                request.UserId,
            }, commandType: CommandType.StoredProcedure);
            return list;
        }

        public async Task<Response> CreatePreprintedQRJob_supplier(PreprintedQRJobSupplierRequest request)
        {
            using var connection = context.CreateConnection();
            var list = await connection.QueryFirstAsync<Response>("CreatePreprintedQRJob_supplier", new
            {
                request.Qty,
                request.ItemCode,
                request.QRType,
                request.UserId,
            }, commandType: CommandType.StoredProcedure);
            return list;
        }

        public async Task<MappingResponse> CreatePreprintedQRMapping(PreprintedQRMappingDBRequest request)
        {
            using var connection = context.CreateConnection();
            var list = await connection.QueryFirstAsync<MappingResponse>("CreatePreprintedQRMapping", new
            {
                request.OuterQRType,
                // request.OuterPrefix,
                request.OuterSrNo,
                request.MasterQRType,
                request.MasterPrefix,
                request.MasterSrNo,
                request.BladeQRType,
                request.BladePrefix,
                request.BladeSrNo,
                request.UserId,
                request.Type,
                request.ItemCode,
            }, commandType: CommandType.StoredProcedure);
            return list;
        }

        public async Task<PreprintedQRDataResponse> GetPreprintedQRJobDetails(int Id)
        {
            using var connection = context.CreateConnection();
            var list = await connection.QueryFirstAsync<PreprintedQRDataResponse>("GetPreprintedQRJobDetails", new
            {
                Id
            }, commandType: CommandType.StoredProcedure);
            return list;
        }

        public async Task<PreprintedQRDataResponse> GetPreprintedQRJobDetails_s(int Id)
        {
            using var connection = context.CreateConnection();
            var list = await connection.QueryFirstAsync<PreprintedQRDataResponse>("GetPreprintedQRJobDetails_s", new
            {
                Id
            }, commandType: CommandType.StoredProcedure);
            return list;
        }

        //public async Task<PreprintedsupplierQRDataResponse> GetPreprintedQRJobDetailsSupplier(int Id)
        //{
        //    using var connection = context.CreateConnection();
        //    var list = await connection.QueryAsync<PreprintedsupplierQRDataResponse>("GetPreprintedQRJobDetailsSupplier", new
        //    {
        //        Id
        //    }, commandType: CommandType.StoredProcedure);
        //    return list;
        //}

        public async Task<IEnumerable<PreprintedsupplierQRDataResponse>> GetPreprintedQRJobDetailsSupplier(int Id)
        {
            using var connection = context.CreateConnection();

            return await connection.QueryAsync<PreprintedsupplierQRDataResponse>(
                "GetPreprintedQRJobDetailsSupplier",
                new { Id },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<Response> UpdatePreprintedQRJobStatus(long id, int userId)
        {
            using var connection = context.CreateConnection();
            var list = await connection.QueryFirstAsync<Response>("UpdatePreprintedQRJobStatus", new
            {
                id,
                userId
            }, commandType: CommandType.StoredProcedure);
            return list;
        }
        public async Task<Response> UpdatePreprintedQRJobStatussupplier(long id, int userId)
        {
            using var connection = context.CreateConnection();
            var list = await connection.QueryFirstAsync<Response>("UpdatePreprintedQRJobStatussupplier", new
            {
                id,
                userId
            }, commandType: CommandType.StoredProcedure);
            return list;
        }

        public async Task<List<PrintedSeq>> GetPrintedSeq()
        {
            using var connection = context.CreateConnection();
            var list = await connection.QueryAsync<PrintedSeq>("GetPrintedSeq", new
            {
                //request.FromDate,
                //request.ToDate
            }, commandType: CommandType.StoredProcedure, commandTimeout: 1200);
            return [.. list];
        }

        public async Task<PreprintedQRReprintResponse> GetPreprintedQrDetailsforReprint(PreprintedQRReprintRequest request)
        {
            using var connection = context.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<PreprintedQRReprintResponse>(
                "GetPreprintedQrDetailsforReprint",
                new
                {
                    request.QRType,
                    request.UserId,
                    request.Srno,
                    request.Prefix
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<PendingPushRecord>> GetPendingPreprintedQRForPush()
        {
            using var connection = context.CreateConnection();

            var list = await connection.QueryAsync<PendingPushRecord>(
                "GetPendingPreprintedQRForPush",
                commandType: CommandType.StoredProcedure);

            return list;
        }


        public async Task<int> UpdatePreprintedQRPushAttempt(string ids)
        {
            using var connection = context.CreateConnection();

            var result = await connection.ExecuteAsync(
                "UpdatePreprintedQRPushAttempt",
                new
                {
                    Ids = ids
                },
                commandType: CommandType.StoredProcedure);

            return result;
        }


        //public async Task<int> DeletePushedPreprintedQR(string ids)
        //{
        //    using var connection = context.CreateConnection();

        //    var result = await connection.ExecuteAsync(
        //        "DeletePushedPreprintedQR",
        //        new
        //        {
        //            Ids = ids
        //        },
        //        commandType: CommandType.StoredProcedure);

        //    return result;
        //}


        public async Task<int> UpdatePreprintedQRPushResponse(
            string ids,
            string response)
        {
            using var connection = context.CreateConnection();

            var result = await connection.ExecuteAsync(
                "UpdatePreprintedQRPushResponse",
                new
                {
                    Ids = ids,
                    Response = response
                },
                commandType: CommandType.StoredProcedure);

            return result;
        }
    }
}
