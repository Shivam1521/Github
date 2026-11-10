using Entities;

namespace IRepository
{
    public interface IDashboardRepository
    {
        Task<DashboardSummary> DashboardSummary(DashboardRequest request);

        Task<DashboardSummaryOverview> GetDashboardOverview(DashboardRequestOverview request);

        Task<DashboardSummaryProductionAnalysis> GetDashboardProductionAnalysis(DashboardRequestProductionAnalysis request);
        
        Task<DashboardSummaryOverviewFilters> GetDashboardOverviewFilters();

        Task<DashboardSummaryProductionAnalysisFilters> GetDashboardProductionAnalysisFilters(int Userid);


        Task<string> GetModuleHHT(int userId);
    }
}
