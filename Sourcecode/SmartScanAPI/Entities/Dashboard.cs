namespace Entities
{
    public class DashboardRequest
    {
        public string FromDate { get; set; } = string.Empty;
        public string ToDate { get; set; } = string.Empty;
        public int UnitId { get; set; }
        public int ToUnitId { get; set; }
    }

    public class DashboardRequestOverview
    {
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }

        public int? UnitId { get; set; }

        public string? LocationType { get; set; }
        public string? Bu { get; set; }
        public string? Section { get; set; }
        public string? Brand { get; set; }
        public string? Category { get; set; }

    }

    public class DashboardRequestProductionAnalysis
    {
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public int? UnitId { get; set; }
        public string? LocationType { get; set; }
        public int? Supervisorid { get; set; }
        public string? Category { get; set; }

    }
    public class DashboardSummaryProductionAnalysis
    {
        public string OvMonth_Wiseproduction { get; set; } = string.Empty;
    }

    public class DashboardSummary
    {
        public string JobDetails { get; set; } = string.Empty;
        public string QRPrinting { get; set; } = string.Empty;
        public string PickSlip { get; set; } = string.Empty;
        public string QCStatusBreakdown { get; set; } = string.Empty;
        public string DispatchReceiving { get; set; } = string.Empty;
    }

   

    public class DashboardSummaryOverview
    {
        public string OvQrPrinting { get; set; } = string.Empty;
        public string OvPlant_Percent { get; set; } = string.Empty;
        public string OvMonth_Wiseproduction { get; set; } = string.Empty;
        public string OvClass_wiseproduction { get; set; } = string.Empty;
    }



    public class DashboardSummaryOverviewFilters
    {

        public string Units { get; set; } = string.Empty;
        public string FYears { get; set; } = string.Empty;
        public string Locationtype { get; set; } = string.Empty;
        public string BuNames { get; set; } = string.Empty;

        public string SectionNames { get; set; } = string.Empty;
        public string BrandNames { get; set; } = string.Empty;
        public string CategoryNames { get; set; } = string.Empty;
    }

    public class DashboardSummaryProductionAnalysisFilters
    {

        public string Units { get; set; } = string.Empty;
        public string FYears { get; set; } = string.Empty;
        public string Locationtype { get; set; } = string.Empty;

        public string Supervisors { get; set; } = string.Empty;
        public string CategoryNames { get; set; } = string.Empty;
    }
    public class DashboardResponseProductionAnalysis
    {
        public string ResponseCode { get; set; } = string.Empty;
        public string ResponseMessage { get; set; } = string.Empty;
        public DashboardDataProductionAnalysis? ResponseData { get; set; } = new();
    }

    public class DashboardResponseProductionAnalysisFilters
    {
        public string ResponseCode { get; set; } = string.Empty;
        public string ResponseMessage { get; set; } = string.Empty;
        public DashboardDataProductionAnalysisFilters? ResponseData { get; set; } = new();
    }

    public class DashboardResponseOverview
    {
        public string ResponseCode { get; set; } = string.Empty;
        public string ResponseMessage { get; set; } = string.Empty;
        public DashboardDataOverview? ResponseData { get; set; } = new();
    }


    public class DashboardResponseOverviewFilters
    {
        public string ResponseCode { get; set; } = string.Empty;
        public string ResponseMessage { get; set; } = string.Empty;
        public DashboardDataOverviewfilters? ResponseData { get; set; } = new();
    }

    public class DashboardResponse
    {
        public string ResponseCode { get; set; } = string.Empty;
        public string ResponseMessage { get; set; } = string.Empty;
        public DashboardData? ResponseData { get; set; } = new();
    }

    public class DashboardData
    {
        public JobDetails? JobDetails { get; set; } = new();
        public QRPrinting? QRPrinting { get; set; } = new();
        public PickSlipDashboard? PickSlip { get; set; } = new();
        public List<QCStatusBreakdown>? QCStatusBreakdown { get; set; } = [];
        public DispatchReceiving? DispatchReceiving { get; set; } = new();
    }

    public class DashboardDataOverviewfilters
    {
        public List<Units>? Units { get; set; } = [];
        public List<FYears>? FYears { get; set; } = [];
        public List<Locationtype>? Locationtype { get; set; } = [];

        public List<BuNames>? BuNames { get; set; } = [];

        public List<SectionNames>? SectionNames { get; set; } = [];
        public List<BrandNames>? BrandNames { get; set; } = [];
        public List<CategoryNames>? CategoryNames { get; set; } = [];

    }

    public class DashboardDataProductionAnalysisFilters
    {
        public List<Units>? Units { get; set; } = [];
        public List<FYears>? FYears { get; set; } = [];
        public List<Locationtype>? Locationtype { get; set; } = [];
        public List<Supervisors>? Supervisors { get; set; } = [];
        public List<CategoryNames>? CategoryNames { get; set; } = [];

    }

    public class Units
    {
        public string UnitName { get; set; } = string.Empty;
        public int UnitId { get; set; }
    }
    public class FYears
    {
        public string FinancialYear { get; set; } = string.Empty;
    }

    public class Locationtype
    {
        public string UnitType { get; set; } = string.Empty;
    }

    public class BuNames
    {
        public string BU { get; set; } = string.Empty;
    }

    public class SectionNames
    {
        public string Section { get; set; } = string.Empty;
    }

    public class BrandNames
    {
        public string Brand { get; set; } = string.Empty;
    }
    public class CategoryNames
    {
        public string Category { get; set; } = string.Empty;
    }

    public class Supervisors
    {
        public int SupervisorId { get; set; }
        public string SupervisorName { get; set; } = string.Empty;
    }

    public class DashboardDataOverview
    {
        public OvQrPrinting? OvQrPrinting { get; set; } = new();
        public List<OvPlant_Percent>? OvPlant_Percent { get; set; } = [];
        public List<OvMonth_Wiseproduction>? OvMonth_Wiseproduction { get; set; } = [];
        public List<OvClass_wiseproduction>? OvClass_wiseproduction { get; set; } = [];
    }

    public class DashboardDataProductionAnalysis
    {
        public List<OvMonth_Wiseproduction>? OvMonth_Wiseproduction { get; set; } = [];
    }

    public class GetDashboardProductionAnalysis
    {
        public List<OvMonth_Wiseproduction>? OvMonth_Wiseproduction { get; set; } = [];
    }

    public class OvQrPrinting
    {
        public int ProductionQty { get; set; }
        public int RejectionQty { get; set; }
        public decimal ProductionPercent { get; set; }
        public decimal RejectionPercent { get; set; }
    }

    public class OvPlant_Percent
    {
        public string PlantName { get; set; } = string.Empty;
        public int Qty { get; set; }
        public decimal ProductionPercentage { get; set; }
    }

    public class OvMonth_Wiseproduction
    {
        public string Month { get; set; } = string.Empty;
        public int Qty { get; set; }
    }
    public class OvClass_wiseproduction
    {
        public string ClassName { get; set; } = string.Empty;
        public int Qty { get; set; }
    }

    public class JobDetails
    {
        public int Completed { get; set; }
        public int Running { get; set; }
    }

    public class QRPrinting
    {
        public int ProductionLine { get; set; }
        public int PrePrint { get; set; }
    }

    public class PickSlipDashboard
    {
        public int ReceivedEBS { get; set; }
        public int ProcessedWH { get; set; }
    }

    public class QCStatusBreakdown
    {
        public string Label { get; set; } = string.Empty;
        public decimal Value { get; set; }
    }

    public class DispatchReceiving
    {
        public int TotalDispatch { get; set; }
        public int TotalReceived { get; set; }
        public List<ReceivedData>? ReceivedData { get; set; } = [];
    }

    public class ReceivedData
    {
        public string Label { get; set; } = string.Empty;
        public Nullable<int> Dispatch { get; set; }
        public Nullable<int> Receive { get; set; }
    }

    public class ModuleData
    {
        public string? Module { get; set; }
    }

    public class ModuleResponse
    {
        public string ResponseCode { get; set; } = string.Empty;
        public string ResponseMessage { get; set; } = string.Empty;
        public List<ModuleData> Modules { get; set; } = new();
    }

}
