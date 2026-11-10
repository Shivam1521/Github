using Entities;

namespace IRepository
{
    public interface IPeripheralDevicesRepository
    {
        Task<Response> CreateUpdatePeripheralDevices(PeripheralDevicesRequest request);
        Task<IEnumerable<PeripheralDevicesResponse>> GetPeripheralDevices(TableRequest request);
        Task<IEnumerable<PeripheralDevicesDDL>> GetPeripheralDevicesDDL(int userId);
    }
}
