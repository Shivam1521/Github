using Dapper;
using Entities;
using IRepository;
using Repository.Data;
using System.Data;

namespace Repository
{
    public class UnitRepository(DapperContext context) : IUnitRepository
    {
        public async Task<Response> CreateUpdateUnit(UnitMasterRequest request)
        {
            using var connection = context.CreateConnection();
            var result = await connection.QueryFirstAsync<Response>("CreateUpdateUnit", new
            {
                request.UnitId,
                //request.LocationId,
                request.UnitName,
                request.UnitCode,
                request.UnitType,
                //request.Email,
                //request.MobileNumber,
                request.Address1,
                request.Address2,
                request.Address3,
                request.State,
                request.City,
                request.Country,
                request.Pincode,
                request.Prefix,
                request.UserId
            }, commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<IEnumerable<UnitMasterResponse>> GetUnits(TableRequest request)
        {
            using var connection = context.CreateConnection();
            var units = await connection.QueryAsync<UnitMasterResponse>("GetUnits", new
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

        public async Task<IEnumerable<DDL>> GetUnitsDDL()
        {
            using var connection = context.CreateConnection();
            var list = await connection.QueryAsync<DDL>("GetUnitsDDL", new
            { }, commandType: CommandType.StoredProcedure);
            return list;
        }
    }
}
