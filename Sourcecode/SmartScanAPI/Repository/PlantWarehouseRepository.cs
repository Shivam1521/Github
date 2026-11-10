using Dapper;
using Entities;
using IRepository;
using Repository.Data;
using System.Data;

namespace Repository
{
    public class PlantWarehouseRepository(DapperContext context) : IPlantWarehouseRepository
    {
        public async Task<Response> CreateUpdatePlantWarehouse(PlantWarehouseRequest request)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryFirstAsync<Response>("CreateUpdatePlantWarehouse", new
            {
                request.PlantWarehouseId,
                request.UnitId,
                request.Name,
                request.Code,
                request.Email,
                request.MobileNumber,
                request.IsPlant,
                request.UserId
            }, commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<IEnumerable<PlantWarehouseResponse>> GetPlantWarehouses(TableRequest request)
        {
            using var connection = context.CreateConnection();
            var units = await connection.QueryAsync<PlantWarehouseResponse>("GetPlantWarehouses", new
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

        public async Task<IEnumerable<DDL>> GetPlantWarehouseDDL(int unitId)
        {
            using var connection = context.CreateConnection();
            var list = await connection.QueryAsync<DDL>("GetPlantWarehouseDDL", new
            {
                unitId
            }, commandType: CommandType.StoredProcedure);
            return list;
        }
    }
}
