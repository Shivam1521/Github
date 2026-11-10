namespace Entities
{
    public class PeripheralDevices
    {
        public int UnitId { get; set; }
        public int DeviceId { get; set; }
        public required int DeviceTypeId { get; set; }
        public required int LineNoId { get; set; }
        public required string Brand { get; set; }
        public required string IP { get; set; }
        public required string Port { get; set; }
        public bool IsActive { get; set; }
    }

    public class PeripheralDevicesRequest : PeripheralDevices
    {
        public int UserId { get; set; }
    }

    public class PeripheralDevicesResponse : PeripheralDevices
    {
        public int TotalRows { get; set; }
        public string DeviceType { get; set; } = string.Empty;
        public string LineNo { get; set; } = string.Empty;
    }

    public class PeripheralDevicesDDL
    {
        public int DeviceId { get; set; }
        public required string DeviceType { get; set; }
        public required string LineNo { get; set; }
        public required string Brand { get; set; }
        public required string IP { get; set; }
        public required string Port { get; set; }
    }
}
