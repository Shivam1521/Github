namespace Entities
{
    public class PlantWarehouse
    {
        public int PlantWarehouseId { get; set; }
        public required int UnitId { get; set; }
        public required string Name { get; set; }
        public required string Code { get; set; }
        public string? Email { get; set; }
        public string? MobileNumber { get; set; }
        public bool IsPlant { get; set; }
    }

    public class PlantWarehouseRequest : PlantWarehouse
    {
        public int UserId { get; set; }
    }

    public class PlantWarehouseResponse : PlantWarehouse
    {
        public string UnitName { get; set; } = string.Empty;
        public int TotalRows { get; set; }
    }
}
