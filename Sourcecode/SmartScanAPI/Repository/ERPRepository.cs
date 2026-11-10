using Dapper;
using Entities;
using IRepository;
using Repository.Data;
using System.Data;

namespace Repository
{
    public class ERPRepository(DapperContext context) : IERPRepository
    {
        public async Task<Response> MergeSKUs(IEnumerable<Items> SKUs, int userId)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("ItemCode", typeof(string));
            dataTable.Columns.Add("Description", typeof(string));
            dataTable.Columns.Add("Brand", typeof(string));
            dataTable.Columns.Add("MFGLocation", typeof(string));
            dataTable.Columns.Add("Class", typeof(string));
            dataTable.Columns.Add("MasterCartonPacking", typeof(int));
            dataTable.Columns.Add("OuterBoxPacking", typeof(int));

            foreach (var item in SKUs)
            {
                dataTable.Rows.Add(item.ItemCode, item.Description, item.Brand, item.MFGLocation, item.Class, item.MasterCartonPackingSize, item.OuterBoxPackingSize);
            }

            var parameters = new DynamicParameters();
            parameters.Add("@SKUs", dataTable.AsTableValuedParameter("dbo.SKU"));
            parameters.Add("@userId", userId);

            using var connection = context.CreateConnection();
            return await connection.QueryFirstAsync<Response>("MergeSKUs", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<Response> MergePlants(IEnumerable<Plants> plants, int userId)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("PlantCode", typeof(string));
            dataTable.Columns.Add("PlantName", typeof(string));
            dataTable.Columns.Add("PlantType", typeof(string));
            dataTable.Columns.Add("AddressLine", typeof(string));
            dataTable.Columns.Add("City", typeof(string));
            dataTable.Columns.Add("State", typeof(string));
            dataTable.Columns.Add("Country", typeof(string));
            dataTable.Columns.Add("PostalCode", typeof(string));

            foreach (var plant in plants)
            {
                dataTable.Rows.Add(plant.PlantCode, plant.PlantName, plant.PlantType, plant.AddressLine, plant.City, plant.State, plant.Country, plant.PostalCode);
            }

            var parameters = new DynamicParameters();
            parameters.Add("@Plants", dataTable.AsTableValuedParameter("dbo.Plant"));
            parameters.Add("@userId", userId);

            using var connection = context.CreateConnection();
            return await connection.QueryFirstAsync<Response>("MergePlants", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<Response> MergeInsertPickSlips(IEnumerable<MergePickSlips> pickSlips, int userId)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("DeliveryDtlId", typeof(string));
            dataTable.Columns.Add("UpdatedOn", typeof(string));
            dataTable.Columns.Add("PickSlipNumber_Child", typeof(string));
            dataTable.Columns.Add("PickSlipNumber_Parent", typeof(string));

            foreach (var item in pickSlips)
            {
                dataTable.Rows.Add( item.DeliveryDtlId, item.UpdatedOn, item.CHILD_PICKSLIP_NUM, item.PARENT_PICKSLIP_NUM);
            }

            var parameters = new DynamicParameters();
            parameters.Add("@MergeDetails", dataTable.AsTableValuedParameter("dbo.MergePickSlipType"));
            parameters.Add("@AddedBy", userId);

            using var connection = context.CreateConnection();
            return await connection.QueryFirstAsync<Response>("MergeInsertPickSlips", parameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<Response> InsertPickSlips(IEnumerable<PickSlips> pickSlips, int userId)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("PickSlipNumber", typeof(string));
            dataTable.Columns.Add("SourceBU", typeof(string));
            dataTable.Columns.Add("ItemCode", typeof(string));
            dataTable.Columns.Add("DeliveryDtlId", typeof(string));
            dataTable.Columns.Add("LineStatus", typeof(string));
            dataTable.Columns.Add("Quantity", typeof(int));
            dataTable.Columns.Add("DestinationBU", typeof(string));
            dataTable.Columns.Add("OrderType", typeof(string));
            dataTable.Columns.Add("CreatedOn", typeof(string));
            dataTable.Columns.Add("UpdatedOn", typeof(string));
            dataTable.Columns.Add("PickSlipNumber_Child", typeof(string));
            dataTable.Columns.Add("PickSlipNumber_Parent", typeof(string));

            foreach (var item in pickSlips)
            {
                dataTable.Rows.Add(item.PickSlipNumber, item.SourceBU, item.ItemCode, item.DeliveryDtlId, item.LineStatus, item.Quantity, item.DestinationBU, item.OrderType, item.CreatedOn, item.UpdatedOn,item.CHILD_PICKSLIP_NUM ,item.PARENT_PICKSLIP_NUM );
            }

            var parameters = new DynamicParameters();
            parameters.Add("@Details", dataTable.AsTableValuedParameter("dbo.PickSlipTypeUpdate"));
            parameters.Add("@AddedBy", userId);

            using var connection = context.CreateConnection();
            return await connection.QueryFirstAsync<Response>("InsertPickSlips", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<Response> InsertUpdateKanban(IEnumerable<Kanban> list, int userId)
        {
            var dt = new DataTable();

            dt.Columns.Add("MfgLocation", typeof(string));
            dt.Columns.Add("ItemCode", typeof(string));
            dt.Columns.Add("LotSize", typeof(long));
            dt.Columns.Add("CardQty", typeof(int));
            dt.Columns.Add("QtyToBeDispatched", typeof(long));
            dt.Columns.Add("Priority", typeof(string));
            dt.Columns.Add("TxnDateTime", typeof(DateTime));
            dt.Columns.Add("MSL", typeof(long));
            dt.Columns.Add("WHOnHand", typeof(long));
            dt.Columns.Add("CardPicked", typeof(int));
            dt.Columns.Add("PickedQty", typeof(long));

            foreach (var k in list)
            {
                dt.Rows.Add(
                    k.MfgLocation,
                    k.ItemCode,
                    k.LotSize,
                    k.CardQty,
                    k.QtyToBeDispatched,
                    k.Priority,
                    k.TxnDateTime, // If string else use k.TxnDateTime directly
                    k.MSL,
                    k.WHOnHand,
                    k.CardPicked,
                    k.PickedQty
                );
            }

            var parameters = new DynamicParameters();
            parameters.Add("@KanbanList", dt.AsTableValuedParameter("dbo.KanbanType"));
            parameters.Add("@AddedBy", userId);

            using var connection = context.CreateConnection();
            return await connection.QueryFirstAsync<Response>("InsertUpdateKanban", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<Response> LogEBSAPIRequestResponse(EBSAPILog log)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryFirstAsync<Response>("LogEBSAPIRequestResponse", new
            {
                log.APIName,
                log.URL,
                log.Request,
                log.Response,
                log.RequestedBy
            });
            return result;
        }
    }
}
