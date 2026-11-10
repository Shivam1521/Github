using Entities;

namespace IRepository
{
    public interface IPlantWarehouseRepository
    {
        Task<Response> CreateUpdatePlantWarehouse(PlantWarehouseRequest request);
        Task<IEnumerable<PlantWarehouseResponse>> GetPlantWarehouses(TableRequest request);
        Task<IEnumerable<DDL>> GetPlantWarehouseDDL(int unitId);
    }
}
