using Dapper;
using Entities;
using IRepository;
using Repository.Data;
using System.Data;

namespace Repository
{
    public class CartonLedgerRepository(DapperContext context) : ICartonLedgerRepository
    {


        public async Task<ReceiveCartonResponse> UpdateReceiveStatus(ReceiveCarton request)
        {
            using var connection = context.CreateConnection();

            var parameters = new DynamicParameters();

            parameters.Add("@SeqNo", request.SeqNo);
            parameters.Add("@Prefix", request.Prefix);
            parameters.Add("@QRType", request.QRType);
            parameters.Add("@ReceivingLocation", request.ReceivingLocation);
            parameters.Add("@PerformedBy", request.PerformedBy);

            var result = await connection.QueryFirstOrDefaultAsync<ReceiveCartonResponse>(
                "ReceiveCarton_In",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }



        public async Task<Response> SubmitReceiveCartons(string pickSlipNumber)
        {
            using var connection = context.CreateConnection();

            var parameters = new DynamicParameters();

            parameters.Add("@PickSlipNumber", pickSlipNumber);

            var result = await connection.QueryFirstOrDefaultAsync<Response>(
                "SubmitReceivedCartons",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<IEnumerable<ReceiveCartonResult>> UpdateReceiveStatusBulk(DataTable table, int performedBy)
        {
            using var connection = context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Cartons", table.AsTableValuedParameter("dbo.ReceiveCartonType"));
            parameters.Add("@PerformedBy", performedBy);

            var result = (await connection.QueryAsync<ReceiveCartonResult>(
                "ReceiveCarton_Bulk",
                parameters,
                commandType: CommandType.StoredProcedure
            )).ToList();
            return result;
        }

        public async Task<List<DispatchCartonResult>> DispatchFullCartons(List<DispatchCartonTVP> cartons, DispatchCartonsRequest request, int performedBy)
        {
            using var connection = context.CreateConnection();

            // Convert List -> DataTable (TVP)
            var table = new DataTable();
            table.Columns.Add("RowId", typeof(int));
            table.Columns.Add("SeqNo", typeof(int));
            table.Columns.Add("Prefix", typeof(string));
            table.Columns.Add("QRType", typeof(string));
            table.Columns.Add("QR", typeof(string));

            foreach (var item in cartons)
            {
                table.Rows.Add(
                    item.RowId,
                    item.SeqNo,
                    item.Prefix,
                    item.QRType,
                    item.QR
                );
            }

            var parameters = new DynamicParameters();

            parameters.Add(
                "@Cartons",
                table.AsTableValuedParameter("dbo.DispatchCartonTVP")
            );

            parameters.Add("@PickSlipNumber", request.PickSlipNumber);
            parameters.Add("@PerformedBy", performedBy);

            var result = await connection.QueryAsync<DispatchCartonResult>(
                "dbo.DispatchFullCartons",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return [.. result];
        }


        public async Task<List<DispatchCartonResult>> Dispatchverify(List<DispatchCartonTVP> cartons, DispatchCartonsRequest request, int performedBy)
        {
            using var connection = context.CreateConnection();

            // Convert List -> DataTable (TVP)
            var table = new DataTable();
            table.Columns.Add("RowId", typeof(int));
            table.Columns.Add("SeqNo", typeof(int));
            table.Columns.Add("Prefix", typeof(string));
            table.Columns.Add("QRType", typeof(string));
            table.Columns.Add("QR", typeof(string));

            foreach (var item in cartons)
            {
                table.Rows.Add(
                    item.RowId,
                    item.SeqNo,
                    item.Prefix,
                    item.QRType,
                    item.QR
                );
            }

            var parameters = new DynamicParameters();

            parameters.Add(
                "@Cartons",
                table.AsTableValuedParameter("dbo.DispatchCartonTVP")
            );

            parameters.Add("@PickSlipNumber", request.PickSlipNumber);
            parameters.Add("@PerformedBy", performedBy);

            var result = await connection.QueryAsync<DispatchCartonResult>(
                "dbo.DispatchFullCartonsTemp",//DispatchFullCartonsTemp
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return [.. result];
        }

        public async Task<Response> SubmitDispatchFullCartons(string pickSlipNumber,int performedBy)
        {
            using var connection = context.CreateConnection();

            var parameters = new DynamicParameters();

            parameters.Add("@PickSlipNumber", pickSlipNumber);
            parameters.Add("@PerformedBy", performedBy);

            var result = await connection.QueryFirstOrDefaultAsync<Response>(
                "dbo.SubmitDispatchFullCartons",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result ?? new Response
            {
                ResponseCode = "400",
                ResponseMessage = "No response received from database."
            };
        }
        public async Task<Response> UpdateDispatchStatus(DispatchCarton request)
        {
            var tvp = new DataTable();
            tvp.Columns.Add("ItemCode", typeof(string));
            tvp.Columns.Add("QR", typeof(string));
            tvp.Columns.Add("Qty", typeof(int));
            tvp.Columns.Add("SeqNo", typeof(long));
            tvp.Columns.Add("Prefix", typeof(string));
            tvp.Columns.Add("QRType", typeof(string));
            tvp.Columns.Add("StatusMessage", typeof(string));

            foreach (var outer in request.Outers)
            {
                tvp.Rows.Add(
                    outer.ItemCode,
                    outer.QR,
                    outer.Qty,
                    outer.SeqNo,
                    outer.Prefix,
                    outer.QRType,
                    outer.StatusMessage
                );
            }

            using var connection = context.CreateConnection();
            var result = await connection.QueryFirstAsync<Response>("DispatchCarton", new
            {
                request.SeqNo,
                request.Prefix,
                request.QRType,
                request.PickSlipNumber,
                request.PerformedBy,
                request.PickType,
                Outers = tvp.AsTableValuedParameter("dbo.OuterDetailsType"),
            }, commandType: CommandType.StoredProcedure);
            return result;
        }

        //public async Task<DemapPickSlipDetailResponse> DemapPickSlipDetail(DemapPickSlipDetailRequestDB request)
        //{
        //    using var connection = context.CreateConnection();
        //    var result = await connection.QueryFirstOrDefaultAsync<DemapPickSlipDetailResponse>("demap_ProcessPickSlipDetail", new
        //    {
        //        request.SeqNo,
        //        request.QRType,
        //        request.Prefix
        //    }, commandType: CommandType.StoredProcedure);
        //    return result;
        //}
    }
}
