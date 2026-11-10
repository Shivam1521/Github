using DocumentFormat.OpenXml.Spreadsheet;
using Entities;
using IRepository;
using Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;

namespace SmartScanAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController(IWrapperRepository repo, ILoggerManager _logger) : ControllerBase
    {
        [HttpPost("GetDashboardData")]
        public async Task<ActionResult<DashboardResponse>> GetDashboardData([FromBody] DashboardRequest request)
        {
            DashboardResponse response = new();
            try
            {
                var dashboardSummary = await repo.Dashboard.DashboardSummary(request);
                response.ResponseCode = "200";
                response.ResponseMessage = "Data Fetched Successfully";
                response.ResponseData ??= new DashboardData();
                response.ResponseData.PickSlip = !string.IsNullOrEmpty(dashboardSummary.PickSlip)
                    ? JsonConvert.DeserializeObject<PickSlipDashboard>(dashboardSummary.PickSlip)
                    : null;
                response.ResponseData.QRPrinting = !string.IsNullOrEmpty(dashboardSummary.QRPrinting)
                    ? JsonConvert.DeserializeObject<QRPrinting>(dashboardSummary.QRPrinting)
                    : null;
                response.ResponseData.JobDetails = !string.IsNullOrEmpty(dashboardSummary.JobDetails)
                    ? JsonConvert.DeserializeObject<JobDetails>(dashboardSummary.JobDetails)
                    : null;
                response.ResponseData.QCStatusBreakdown = !string.IsNullOrEmpty(dashboardSummary.QCStatusBreakdown)
                    ? JsonConvert.DeserializeObject<List<QCStatusBreakdown>>(dashboardSummary.QCStatusBreakdown)
                    : null;
                response.ResponseData.DispatchReceiving = !string.IsNullOrEmpty(dashboardSummary.DispatchReceiving)
                    ? JsonConvert.DeserializeObject<DispatchReceiving>(dashboardSummary.DispatchReceiving)
                    : null;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogInfo("GetDashboardData Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("GetDashboardData Error: " + JsonConvert.SerializeObject(ex));
                return StatusCode(500, "Internal Server Error, Please Try again later or contact admin");
            }
        }

        [HttpPost("GetDashboardProductionAnalysis")]
        public async Task<ActionResult<DashboardResponseProductionAnalysis>> GetDashboardProductionAnalysis([FromBody] DashboardRequestProductionAnalysis request)
        {
            DashboardResponseProductionAnalysis response = new();
            try
            {
                var dashboardSummary = await repo.Dashboard.GetDashboardProductionAnalysis(request);
                response.ResponseCode = "200";
                response.ResponseMessage = "Data Fetched Successfully";
                response.ResponseData ??= new DashboardDataProductionAnalysis();
                response.ResponseData.OvMonth_Wiseproduction = !string.IsNullOrEmpty(dashboardSummary.OvMonth_Wiseproduction)
                    ? JsonConvert.DeserializeObject<List<OvMonth_Wiseproduction>>(dashboardSummary.OvMonth_Wiseproduction)
                    : null;
              
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogInfo("GetDashboardProductionAnalysis Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("GetDashboardProductionAnalysis Error: " + JsonConvert.SerializeObject(ex));
                return StatusCode(500, "Internal Server Error, Please Try again later or contact admin");
            }
        }

        [HttpPost("GetDashboardProductionAnalysisFilters")]
        public async Task<ActionResult<DashboardResponseProductionAnalysisFilters>> GetDashboardProductionAnalysisFilters()
        {
            DashboardResponseProductionAnalysisFilters response = new();
            try
            {
                var Userid = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);
                var dashboardSummary = await repo.Dashboard.GetDashboardProductionAnalysisFilters(Userid);
                response.ResponseCode = "200";
                response.ResponseMessage = "Data Fetched Successfully";
                response.ResponseData ??= new DashboardDataProductionAnalysisFilters();
                response.ResponseData.Units = !string.IsNullOrEmpty(dashboardSummary.Units)
                    ? JsonConvert.DeserializeObject<List<Units>>(dashboardSummary.Units)
                    : null;
                response.ResponseData.FYears = !string.IsNullOrEmpty(dashboardSummary.FYears)
                    ? JsonConvert.DeserializeObject<List<FYears>>(dashboardSummary.FYears)
                    : null;
                response.ResponseData.Locationtype = !string.IsNullOrEmpty(dashboardSummary.Locationtype)
                    ? JsonConvert.DeserializeObject<List<Locationtype>>(dashboardSummary.Locationtype)
                    : null;
                response.ResponseData.Supervisors = !string.IsNullOrEmpty(dashboardSummary.Supervisors)
                   ? JsonConvert.DeserializeObject<List<Supervisors>>(dashboardSummary.Supervisors)
                   : null;
                response.ResponseData.CategoryNames = !string.IsNullOrEmpty(dashboardSummary.CategoryNames)
                   ? JsonConvert.DeserializeObject<List<CategoryNames>>(dashboardSummary.CategoryNames)
                   : null;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogInfo("GetDashboardProductionAnalysisFilters Request: " + JsonConvert.SerializeObject(""));
                _logger.LogError("GetDashboardProductionAnalysisFilters Error: " + JsonConvert.SerializeObject(ex));
                return StatusCode(500, "Internal Server Error, Please Try again later or contact admin");
            }
        }


        [HttpPost("GetDashboardOverview")]
        public async Task<ActionResult<DashboardResponseOverview>> GetDashboardOverview([FromBody] DashboardRequestOverview request)
        {
            DashboardResponseOverview response = new();
            try
            {
                var dashboardSummary = await repo.Dashboard.GetDashboardOverview(request);
                response.ResponseCode = "200";
                response.ResponseMessage = "Data Fetched Successfully";
                response.ResponseData ??= new DashboardDataOverview();
                response.ResponseData.OvQrPrinting = !string.IsNullOrEmpty(dashboardSummary.OvQrPrinting)
                    ? JsonConvert.DeserializeObject<OvQrPrinting>(dashboardSummary.OvQrPrinting)
                    : null;
                response.ResponseData.OvPlant_Percent = !string.IsNullOrEmpty(dashboardSummary.OvPlant_Percent)
                    ? JsonConvert.DeserializeObject<List<OvPlant_Percent>>(dashboardSummary.OvPlant_Percent)
                    : null;
                response.ResponseData.OvMonth_Wiseproduction = !string.IsNullOrEmpty(dashboardSummary.OvMonth_Wiseproduction)
                    ? JsonConvert.DeserializeObject<List<OvMonth_Wiseproduction>>(dashboardSummary.OvMonth_Wiseproduction)
                    : null;
                response.ResponseData.OvClass_wiseproduction = !string.IsNullOrEmpty(dashboardSummary.OvClass_wiseproduction)
                    ? JsonConvert.DeserializeObject<List<OvClass_wiseproduction>>(dashboardSummary.OvClass_wiseproduction)
                    : null;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogInfo("GetDashboardOverview Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("GetDashboardOverview Error: " + JsonConvert.SerializeObject(ex));
                return StatusCode(500, "Internal Server Error, Please Try again later or contact admin");
            }
        }

        [HttpPost("GetDashboardOverviewFilters")]
        public async Task<ActionResult<DashboardResponseOverviewFilters>> GetDashboardOverviewFilters()
        {
            DashboardResponseOverviewFilters response = new();
            try
            {
                var dashboardSummary = await repo.Dashboard.GetDashboardOverviewFilters();
                response.ResponseCode = "200";
                response.ResponseMessage = "Data Fetched Successfully";
                response.ResponseData ??= new DashboardDataOverviewfilters();
                response.ResponseData.Units = !string.IsNullOrEmpty(dashboardSummary.Units)
                    ? JsonConvert.DeserializeObject<List<Units>>(dashboardSummary.Units)
                    : null;
                response.ResponseData.FYears = !string.IsNullOrEmpty(dashboardSummary.FYears)
                    ? JsonConvert.DeserializeObject<List<FYears>>(dashboardSummary.FYears)
                    : null;
                response.ResponseData.Locationtype = !string.IsNullOrEmpty(dashboardSummary.Locationtype)
                    ? JsonConvert.DeserializeObject<List<Locationtype>>(dashboardSummary.Locationtype)
                    : null;
                response.ResponseData.BuNames = !string.IsNullOrEmpty(dashboardSummary.BuNames)
                    ? JsonConvert.DeserializeObject<List<BuNames>>(dashboardSummary.BuNames)
                    : null;
                response.ResponseData.SectionNames = !string.IsNullOrEmpty(dashboardSummary.SectionNames)
                   ? JsonConvert.DeserializeObject<List<SectionNames>>(dashboardSummary.SectionNames)
                   : null;
                response.ResponseData.BrandNames = !string.IsNullOrEmpty(dashboardSummary.BrandNames)
                   ? JsonConvert.DeserializeObject<List<BrandNames>>(dashboardSummary.BrandNames)
                   : null;
                response.ResponseData.CategoryNames = !string.IsNullOrEmpty(dashboardSummary.CategoryNames)
                   ? JsonConvert.DeserializeObject<List<CategoryNames>>(dashboardSummary.CategoryNames)
                   : null;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogInfo("GetDashboardOverviewFilters Request: " + JsonConvert.SerializeObject(""));
                _logger.LogError("GetDashboardOverviewFilters Error: " + JsonConvert.SerializeObject(ex));
                return StatusCode(500, "Internal Server Error, Please Try again later or contact admin");
            }
        }

        [HttpGet("GetModulesHHT")]
        public async Task<ActionResult<ModuleResponse>> GetModulesHHT()
        {
            var res = new ModuleResponse();

            try
            {
                var sidClaim = User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value;

                if (string.IsNullOrEmpty(sidClaim))
                {
                    return Unauthorized("Invalid Token");
                }

                int userId = Convert.ToInt32(sidClaim);

                var jsonResult = await repo.Dashboard.GetModuleHHT(userId);

                List<ModuleData> modulesList = new();

                if (!string.IsNullOrEmpty(jsonResult))
                {
                    try
                    {
                        modulesList = JsonConvert.DeserializeObject<List<ModuleData>>(jsonResult) ?? new();
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError("JSON Error: " + ex.Message);
                    }
                }

                res.ResponseCode = "200";
                res.ResponseMessage = "Success";
                res.Modules = modulesList;

                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetModulesHHT Error"  + JsonConvert.SerializeObject(ex));

                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }
    }
}
