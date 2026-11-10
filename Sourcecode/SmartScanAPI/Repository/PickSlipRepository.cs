using Dapper;
using Entities;
using IRepository;
using Repository.Data;
using System.Data;

namespace Repository
{
    public class PickSlipRepository(DapperContext context) : IPickSlipRepository
    {
        public async Task<IEnumerable<PickSlipDetails>> GetPickSlipDetails(string pickslipNumber)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryAsync<PickSlipDetails>("GetPickSlipDetails", new
            {
                pickslipNumber
            }, commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<PickSlip> GetPickSlipDetailsForPickup(string pickslipNumber)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryFirstAsync<PickSlip>("GetPickSlipDetailsForPickup", new
            {
                pickslipNumber
            }, commandType: CommandType.StoredProcedure);
            return result;
        }
        public async Task<Received> GetDetailsForReceving(string PickSlipNumber)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryFirstAsync<Received>("GetDetailsForReceving", new
            {
                PickSlipNumber
            }, commandType: CommandType.StoredProcedure);
            return result;
        }


        public async Task<PickupQRDetailsResponse> VerifyQRForPickslip(PickupQRDetailsRequestDB request)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryAsync<PickupQRDetailsResponse>("VerifyQRForPickslip", new
            {
                request.SeqNo,
                request.Prefix,
                request.QRType,
                request.PickSlipNumber,
                request.Loose
            }, commandType: CommandType.StoredProcedure);
            return result.First();
        }

        public async Task<Response> InsertPickSlipDetails(InsertPickSlipRequest request)
        {
            var table = new DataTable();
            table.Columns.Add("SeqNo", typeof(long));
            table.Columns.Add("QRType", typeof(string));
            table.Columns.Add("UnitId", typeof(int));
            table.Columns.Add("ItemCode", typeof(string));
            table.Columns.Add("DeliveryDtlId", typeof(string));
            table.Columns.Add("Quantity", typeof(int));

            foreach (var item in request.PickSlipDetails)
            {
                table.Rows.Add(item.SeqNo, item.QRType, item.UnitId, item.ItemCode, item.DeliveryDtlId, item.Quantity);
            }

            var parameters = new DynamicParameters();
            parameters.Add("@PickSlipNumber", request.PickSlipNumber);
            parameters.Add("@AddedBy", request.AddedBy);
            parameters.Add("@PickSlipDetails", table.AsTableValuedParameter("dbo.PickSlipDetailsType"));

            using var connection = context.CreateConnection();

            var result = await connection.QueryFirstAsync<Response>(
                "dbo.InsertPickSlipDetails",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<IEnumerable<PickupQRDetails>> GetTempPickslipQRs(string pickslipNumber)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryAsync<PickupQRDetails>("GetTempPickslipQRs", new
            {
                pickslipNumber
            }, commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<IEnumerable<PickupQRDetails>> GetTempRecivedQRs(string pickslipNumber)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryAsync<PickupQRDetails>("GetTempRecivedQRs", new
            {
                pickslipNumber
            }, commandType: CommandType.StoredProcedure);
            return result;
        }



        public async Task<IEnumerable<PickSlipReportDetails>> GetPickedslipsDetails(TableRequest request)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryAsync<PickSlipReportDetails>("GetPickedslipsDetails", new
            {
                request.PageNo,
                request.PageSize,
                request.SearchValue,
                request.SortColumn,
                request.SortOrder,
                request.UnitId,
                request.FromDate,
                request.ToDate
            }, commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<IEnumerable<PickSlipReceiptStatus>> GetPickSlipReceiptStatus(string pickslipNumber)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryAsync<PickSlipReceiptStatus>("GetPickSlipReceiptStatus", new
            {
                pickslipNumber
            }, commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<DemapPickSlipDetailResponse> DemapPickSlipDetail(DemapPickSlipDetailRequestDB request)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<DemapPickSlipDetailResponse>("demap_ProcessPickSlipDetail", new
            {
                request.SeqNo,
                request.QRType,
                request.Prefix
            }, commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<IEnumerable<PendingPickslipResponse>> GetPendingPickslipsForSubmit(int userId)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryAsync<PendingPickslipResponse>("GetPendingPickslipsForSubmit", new
            {
                userId
            }, commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<IEnumerable<BulkSubmitPickSlipResult>> BulkSubmitPickSlips(BulkSubmitPickSlipsRequestDB request)
        {
            var table = new DataTable();
            table.Columns.Add("PickSlipNumber", typeof(string));
            foreach (var slipNo in request.PickSlipNumbers)
            {
                table.Rows.Add(slipNo);
            }

            var parameters = new DynamicParameters();
            parameters.Add("@PickSlipNumbers", table.AsTableValuedParameter("[PickSlipNumberListType]"));
            parameters.Add("@UserId", request.UserId);  

            using var connection = context.CreateConnection();
            var result = await connection.QueryAsync<BulkSubmitPickSlipResult>(
                "BulkSubmitPickSlips",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result;
        }

    }
}
