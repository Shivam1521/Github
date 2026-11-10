using Dapper;
using Entities;
using IRepository;
using Repository.Data;
using System.Data;

namespace Repository
{
    public class PeripheralDevicesRepository(DapperContext context) : IPeripheralDevicesRepository
    {
        public async Task<Response> CreateUpdatePeripheralDevices(PeripheralDevicesRequest request)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryFirstAsync<Response>("AddUpdatePeripheralDeviceMaster", new
            {
                request.DeviceId,
                request.DeviceTypeId,
                request.LineNoId,
                request.Brand,
                request.IP,
                request.Port,
                request.IsActive,
                request.UserId,
                request.UnitId
            }, commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<IEnumerable<PeripheralDevicesResponse>> GetPeripheralDevices(TableRequest request)
        {
            using var connection = context.CreateConnection();
            var devices = await connection.QueryAsync<PeripheralDevicesResponse>("GetPeripheralDevices", new
            {
                request.PageNo,
                request.PageSize,
                request.SearchValue,
                request.SortColumn,
                request.SortOrder,
                request.UnitId
            }, commandType: CommandType.StoredProcedure);
            return devices;
        }

        public async Task<IEnumerable<PeripheralDevicesDDL>> GetPeripheralDevicesDDL(int userId)
        {
            using var connection = context.CreateConnection();
            var list = await connection.QueryAsync<PeripheralDevicesDDL>("GetPeripheralDevicesDDL", new
            {
                userId
            }, commandType: CommandType.StoredProcedure);
            return list;
        }
    }
}
