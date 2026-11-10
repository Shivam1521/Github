using Dapper;
using Entities;
using IRepository;
using Repository.Data;
using System.Data;

namespace Repository
{
    public class ItemRepository(DapperContext context) : IItemRepository
    {
        public async Task<IEnumerable<ItemDDL>> GetItems(int userId)
        {
            using var connection = context.CreateConnection();
            var list = await connection.QueryAsync<ItemDDL>("GetItems", new
            {
                userId
            }, commandType: CommandType.StoredProcedure);
            return list;
        }

        public async Task<IEnumerable<ItemDetailsResponse>> GetItemDetails(int id, int userId)
        {
            using var connection = context.CreateConnection();
            var list = await connection.QueryAsync<ItemDetailsResponse>("GetItemDetails", new
            {
                id,
                userId
            }, commandType: CommandType.StoredProcedure);
            return list;
        }

        public async Task<IEnumerable<ItemMasterResponse>> GetItemMasterByUserId(TableRequest request)
        {
            using var connection = context.CreateConnection();
            var list = await connection.QueryAsync<ItemMasterResponse>("GetItemMasterByUserId", new
            {
                request.PageNo,
                request.PageSize,
                request.SearchValue,
                request.SortColumn,
                request.SortOrder,
                request.UnitId
            }, commandType: CommandType.StoredProcedure, commandTimeout: 300);
            return list;
        }

        public async Task<Response> UpdateItemMaster(ItemMasterRequest request, int createdby)
        {
            try
            {
                using var connection = context.CreateConnection();

                var result = await connection.QueryFirstOrDefaultAsync<Response>("UpdateItemMaster", new
                {
                    request.Id,
                    request.OuterPerCarton,
                    request.LabelPerLot,
                    request.Pic1LineSupervisor,
                    request.Pic2Engineer,
                    request.LotQty,
                    request.TotalCards,
                    request.TotalRooms,
                    request.CartonPerOuterBoxInLot,
                    request.WHZoneID,
                    Lines = request.Lines.AsTableValuedParameter("dbo.Lines", ["LineId"]),
                    request.PlaceSerial,
                    request.Location,
                    request.ProductionZone,
                    request.Priority,
                    request.Samriddhi,
                    request.SmartSaver,
                    request.OuterBoxMinWeight,
                    request.OuterBoxMaxWeight,
                    UpdatedBy = createdby,
                    request .MasterCartonPackingSize,
                    request.OuterBoxPackingSize
                }, commandType: CommandType.StoredProcedure);

                return result ?? new Response { ResponseMessage = "No record updated." };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    ResponseMessage = $"An error occurred: {ex.Message}"
                };
            }
        }

        public async Task<Response> MapItemsAndUsers(DataTable tvp)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Dump", tvp.AsTableValuedParameter("dbo.TVP_ItemUserDumpupdate"));

            using var connection = context.CreateConnection();
            return await connection.QueryFirstAsync<Response>("MapItemsAndUsers", parameters,
                commandType: CommandType.StoredProcedure, commandTimeout: 6000);
        }
    }
}
