using Dapper;
using Entities;
using IRepository;
using Repository.Data;
using System.Data;

namespace Repository
{
    public class PrintingDetailsRepository(DapperContext context) : IPrintingDetailsRepository
    {
        public async Task<bool> SavePrintingDetailsAsync(List<PrintingDetailsRequest> printingDetailsRequest)
        {
            var connection = context.CreateConnection();
            var table = new DataTable();
            table.Columns.Add("ItemCode", typeof(string));
            table.Columns.Add("Description", typeof(string));
            table.Columns.Add("LotSizeQty", typeof(int));
            table.Columns.Add("NoOfBoxes", typeof(int));
            table.Columns.Add("NoOfCards", typeof(int));
            table.Columns.Add("QtyToBeDispatched", typeof(int));
            table.Columns.Add("Brand", typeof(string));
            table.Columns.Add("SubInv", typeof(string));
            table.Columns.Add("MSL", typeof(string));
            table.Columns.Add("TransactionDate", typeof(DateTime));
            table.Columns.Add("IsPrint", typeof(bool));
            table.Columns.Add("ItemName", typeof(string));
            table.Columns.Add("PrintSourceId", typeof(int));

            foreach (var item in printingDetailsRequest)
            {
                table.Rows.Add(
                    item.ItemCode,
                    item.Description,
                    item.LotSizeQty ?? (object)DBNull.Value,
                    item.NoOfBoxes ?? (object)DBNull.Value,
                    item.NoOfCards ?? (object)DBNull.Value,
                    item.QtyToBeDispatched ?? (object)DBNull.Value,
                    item.Brand,
                    item.SubInv,
                    item.MSL,
                    item.TransactionDate ?? (object)DBNull.Value,
                    item.IsPrint ?? false,
                    item.ItemName,
                    item.PrintSourceId
                );
            }
            var parameters = new DynamicParameters();
            parameters.Add("@ListDetailsTVP", table.AsTableValuedParameter("dbo.TVP_ListDetails"));

            var result = await connection.ExecuteAsync("dbo.SavePrintingDetails", parameters, commandType: CommandType.StoredProcedure);
            return result > 0;

        }
        public async Task<int> SavePrintedData(SavePrintedDataRequest request)
        {
            using var connection = context.CreateConnection();
            var table = new DataTable();
            table.Columns.Add("PrintId", typeof(long));
            table.Columns.Add("PrintSourceId", typeof(long));
            table.Columns.Add("ItemCode", typeof(string));
            table.Columns.Add("ItemName", typeof(string));
            table.Columns.Add("Description", typeof(string));
            table.Columns.Add("LotSizeQty", typeof(int));
            table.Columns.Add("NoOfBoxes", typeof(int));
            table.Columns.Add("NoOfCards", typeof(int));
            table.Columns.Add("QtyToBeDispatched", typeof(int));
            table.Columns.Add("ParcelNumber", typeof(int));
            table.Columns.Add("Brand", typeof(string));
            table.Columns.Add("SubInv", typeof(string));
            table.Columns.Add("MSL", typeof(string));
            table.Columns.Add("TransactionDate", typeof(DateTime));

            foreach (var item in request.PrintIds)
            {
                table.Rows.Add(
                    item.PrintId,
                    item.PrintSourceId,
                    item.ItemCode,
                    item.ItemName,
                    item.Description,
                    item.LotSizeQty,
                    item.NoOfBoxes,
                    item.NoOfCards,
                    item.QtyToBeDispatched,
                    item.ParcelNumber,
                    item.Brand,
                    item.SubInv,
                    item.MSL,
                    item.TransactionDate
                );
            }

            var parameters = new DynamicParameters();
            parameters.Add("@tvpPrinting", table.AsTableValuedParameter("tvp_PrintingData"));
            parameters.Add("@PrintedBy", request.PrintedBy);

            var result = await connection.ExecuteScalarAsync<int>(
                "usp_SavePrintedData",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }
        public async Task<IEnumerable<PrintingDetailsResponse>> GetPrintingDetails()
        {
            using var connection = context.CreateConnection();
            var printingDetails = await connection.QueryAsync<PrintingDetailsResponse>("usp_GetPrintingDetailsByIds", commandType: CommandType.StoredProcedure);
            return printingDetails;
        }
        public async Task<int> SaveMasterPrinting(MasterPrintingRequest request)
        {
            var connection = context.CreateConnection();
            var table = new DataTable();
            table.Columns.Add("PrintId", typeof(int));
            if (request.PrintIds != null)
            {
                foreach (var id in request.PrintIds)
                {
                    table.Rows.Add(id.PrintId);
                }
            }
            var parameters = new DynamicParameters();
            parameters.Add("@tvpPrintIds", table.AsTableValuedParameter("dbo.tvp_PrintIds "));
            parameters.Add("@PrintBy", request.PrintBy);
            var result = await connection.QueryFirstAsync<int>(
                "usp_PrintMasterPacking",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result;
        }

        //public async Task<IEnumerable<PrintList>> GetPrintList()
        //{
        //    using var connection = context.CreateConnection();
        //    var printingDetails = await connection.QueryAsync<PrintList>("select * from W_SerialUpload");
        //    return printingDetails;
        //}

        public async Task<IEnumerable<PrintList>> GetPrintList(string type, int createdBy)
        {
            using var connection = context.CreateConnection();

            var printingDetails = await connection.QueryAsync<PrintList>("GetPrintList", new
            {
                Type = type,
                createdBy
            }, commandType: CommandType.StoredProcedure);

            return printingDetails;
        }

    }
}
