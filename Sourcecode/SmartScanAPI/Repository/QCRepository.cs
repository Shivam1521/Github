using Azure.Core;
using Dapper;
using Entities;
using IRepository;
using Repository.Data;
using System.Data;

namespace Repository
{
    public class QCRepository(DapperContext context) : IQCRepository
    {
        public async Task<CartonDetails> GetCartonDetails(string prefix, long seqNo, string qrType, int actionType)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryFirstAsync<CartonDetails>("GetCartonDetails", new
            {
                prefix,
                seqNo,
                qrType,
                actionType
            }, commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<Response> ReplaceOuterForQC(ReplaceCartonRequest request)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryFirstAsync<Response>("ReplaceOuterForQC", new
            {
                request.FirstSeqNo,
                request.FirstQRType,
                request.FirstUnitId,
                request.SecondSeqNo,
                request.SecondQRType,
                request.SecondUnitId,
                request.PerformedBy
            }, commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<QCStatusDetails> GetQCStatus(string prefix, long seqNo, string qrType)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryFirstAsync<QCStatusDetails>("GetQCStatus", new
            {
                prefix,
                seqNo,
                qrType,
            }, commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<Response> MarkQCOpen(MarkQCOpenRequest request)
        {
            var table = new DataTable();
            table.Columns.Add("SeqNo", typeof(long));
            table.Columns.Add("UnitId", typeof(int));
            table.Columns.Add("QRType", typeof(string));


            foreach (var item in request.QRInfo)
            {
                table.Rows.Add(item.SeqNo, item.UnitId, item.QRType);
            }

            var parameters = new DynamicParameters();
            parameters.Add("@PerformedBy", request.PerformedBy);
            parameters.Add("@QRInfo", table.AsTableValuedParameter("dbo.QRInfoType"));

            using var connection = context.CreateConnection();

            var result = await connection.QueryFirstAsync<Response>(
                "MarkQCOpen",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<JobDetailsForQC> GetJobDetailsForQC(string prefix, long seqNo, string qrType)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryFirstAsync<JobDetailsForQC>("GetJobDetailsForQC", new
            {
                prefix,
                seqNo,
                qrType,
            }, commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<Response> UpdateQCStatus(UpdateQCStatusRequest request)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryFirstAsync<Response>("UpdateQCStatusByJobId", new
            {
                request.JobId,
                request.QCStatus,
                request.PerformedBy,
            }, commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<CartonDetailsForQC> GetCartonDetailsForQC(string prefix, long seqNo, string qrType, int actionType)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryFirstAsync<CartonDetailsForQC>("GetCartonDetailsForQC", new
            {
                prefix,
                seqNo,
                qrType,
                actionType
            }, commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<Response> ReplaceOuterAfterQC(ReplaceOuterAfterQCRequest request)
        {
            var table = new DataTable();
            table.Columns.Add("SeqNo", typeof(long));
            table.Columns.Add("UnitId", typeof(int));
            table.Columns.Add("QRType", typeof(string));

            foreach (var item in request.QRInfo)
            {
                table.Rows.Add(item.SeqNo, item.UnitId, item.QRType);
            }

            var parameters = new DynamicParameters();
            parameters.Add("@MasterSeqNo", request.MasterSeqNo);
            parameters.Add("@MasterQRType", request.MasterQRType);
            parameters.Add("@MasterUnitId", request.MasterUnitId);
            parameters.Add("@QRInfo", table.AsTableValuedParameter("dbo.QRInfoType"));
            parameters.Add("@PerformedBy", request.PerformedBy);

            using var connection = context.CreateConnection();

            var result = await connection.QueryFirstAsync<Response>(
                "ReplaceOuterAfterQC",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<Response> AttachDetachCartons(ReplaceCartonRequest request)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryFirstAsync<Response>("AttachDetachCartons", new
            {
                request.FirstSeqNo,
                request.FirstQRType,
                request.FirstUnitId,
                request.SecondSeqNo,
                request.SecondQRType,
                request.SecondUnitId,
                request.PerformedBy
            }, commandType: CommandType.StoredProcedure);
            return result;
        }
    }
}
