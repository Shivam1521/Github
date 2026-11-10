using Dapper;
using Entities;
using IRepository;
using Repository.Data;
using System.Data;

namespace Repository
{
    public class ListRepository(DapperContext context) : IListRepository
    {
        public async Task<IEnumerable<DDL>> GetList(string listName, int? unitId)
        {
            using var connection = context.CreateConnection();
            var list = await connection.QueryAsync<DDL>("GetList", new
            {
                listName,
                unitId
            }, commandType: CommandType.StoredProcedure);
            return list;
        }

        public async Task<IEnumerable<ListMaster>> GetListHeaders()
        {
            using var connection = context.CreateConnection();
            var list = await connection.QueryAsync<ListMaster>("Select ListId, ListName from ListMaster", new { });
            return list;
        }

        public async Task<IEnumerable<ListDetailResponse>> GetListDetails(TableRequest request)
        {
            using var connection = context.CreateConnection();
            var list = await connection.QueryAsync<ListDetailResponse>("GetListDetails", new
            {
                request.PageNo,
                request.PageSize,
                request.SearchValue,
                request.SortColumn,
                request.SortOrder,
                request.UnitId
            }, commandType: CommandType.StoredProcedure);
            return list;
        }

        public async Task<Response> AddUpdateListMaster(ListDetail detail)
        {
            using var connection = context.CreateConnection();
            var list = await connection.QueryFirstAsync<Response>("AddUpdateListMaster", new
            {
                detail.ListMasterId,
                detail.ListDetailId,
                detail.ListDetailName,
                detail.IsActive,
                detail.UnitId,
                detail.UserId
            }, commandType: CommandType.StoredProcedure);
            return list;
        }
    }
}
