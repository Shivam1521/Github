using Azure.Core;
using Dapper;
using Entities;
using IRepository;
using Repository.Data;
using System.Data;

namespace Repository
{
    public class DashboardRepository(DapperContext context) : IDashboardRepository
    {
        public async Task<DashboardSummary> DashboardSummary(DashboardRequest request)
        {
            using var connection = context.CreateConnection();
            var users = await connection.QueryFirstAsync<DashboardSummary>("GetDashboardSummary", new
            {
                request.FromDate,
                request.ToDate,
                request.UnitId,
                request.ToUnitId
            }, commandType: CommandType.StoredProcedure);
            return users;
        }

        public async Task<DashboardSummaryProductionAnalysis> GetDashboardProductionAnalysis(DashboardRequestProductionAnalysis request)
        {
            try
            {
                using var connection = context.CreateConnection();

                var users = await connection.QueryFirstAsync<DashboardSummaryProductionAnalysis>(
                    "GetDashboardProductionAnalysis",
                    new
                    {
                        FromDate = string.IsNullOrWhiteSpace(request.FromDate) || request.FromDate == "string"
                                    ? null : request.FromDate,

                        ToDate = string.IsNullOrWhiteSpace(request.ToDate) || request.ToDate == "string"
                                    ? null : request.ToDate,

                        UnitId = request.UnitId == 0 ? (int?)null : request.UnitId,

                        LocationType = string.IsNullOrWhiteSpace(request.LocationType) || request.LocationType == "string"
                                    ? null : request.LocationType,

                        Supervisorid = request.Supervisorid == 0 ? (int?)null : request.Supervisorid,

                        Category = string.IsNullOrWhiteSpace(request.Category) || request.Category == "string"
                                    ? null : request.Category
                    },
                    commandType: CommandType.StoredProcedure
                );

                return users;
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching dashboard overview.", ex);
            }
        }

        public async Task<DashboardSummaryOverview> GetDashboardOverview(DashboardRequestOverview request)
        {
            try
            {
                using var connection = context.CreateConnection();

                var users = await connection.QueryFirstAsync<DashboardSummaryOverview>(
                    "GetDashboardOverview",
                    new
                    {
                        FromDate = string.IsNullOrWhiteSpace(request.FromDate) || request.FromDate == "string"
                                    ? null : request.FromDate,

                        ToDate = string.IsNullOrWhiteSpace(request.ToDate) || request.ToDate == "string"
                                    ? null : request.ToDate,

                        UnitId = request.UnitId == 0 ? (int?)null : request.UnitId,

                        LocationType = string.IsNullOrWhiteSpace(request.LocationType) || request.LocationType == "string"
                                    ? null : request.LocationType,

                        Bu = string.IsNullOrWhiteSpace(request.Bu) || request.Bu == "string"
                                    ? null : request.Bu,

                        Section = string.IsNullOrWhiteSpace(request.Section) || request.Section == "string"
                                    ? null : request.Section,

                        Brand = string.IsNullOrWhiteSpace(request.Brand) || request.Brand == "string"
                                    ? null : request.Brand,

                        Category = string.IsNullOrWhiteSpace(request.Category) || request.Category == "string"
                                    ? null : request.Category
                    },
                    commandType: CommandType.StoredProcedure
                );

                return users;
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching dashboard overview.", ex);
            }
        }


        public async Task<DashboardSummaryOverviewFilters> GetDashboardOverviewFilters()
        {
          
                using var connection = context.CreateConnection();

                var users = await connection.QueryFirstAsync<DashboardSummaryOverviewFilters>(
                    "GetDashboardOverviewFilters",
                    new
                    {},
                    commandType: CommandType.StoredProcedure
                );

                return users;
        }

        public async Task<DashboardSummaryProductionAnalysisFilters> GetDashboardProductionAnalysisFilters(int Userid)
        {

            using var connection = context.CreateConnection();

            var users = await connection.QueryFirstAsync<DashboardSummaryProductionAnalysisFilters>(
                "GetDashboardProductionAnalysisFilters",
                new
                { Userid = Userid == 0 ? (int?)null : Userid },
                commandType: CommandType.StoredProcedure
            );

            return users;
        }


        public async Task<string> GetModuleHHT(int userId)
        {
            using var connection = context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Userid", userId, DbType.Int32);

            var result = await connection.QueryFirstOrDefaultAsync<string>(
                "GetModuleHHT",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }
    }
}
