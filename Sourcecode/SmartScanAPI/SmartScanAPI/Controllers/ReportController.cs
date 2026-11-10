using Entities;
using IRepository;
using Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
namespace SmartScanAPI.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    [Authorize]
    public class ReportController(ILoggerManager _logger, IWrapperRepository repo) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<HttpResponse<SerialReport>>> GetSerialUploadReport(TableRequest request)
        {
            HttpResponse<SerialReport> res = new();
            try
            {
                var result = await repo.AllReport.GetSerialUploadReport(request);
                if (result?.Any() == true)
                {
                    res.ResponseCode = "200";
                    res.ResponseMessage = "Success";
                    res.ResponseData = result;
                    return Ok(res);
                }
                else
                {
                    res.ResponseCode = "404";
                    res.ResponseMessage = "No Data Found";
                    return NotFound(res);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("GetPrintList Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("GetPrintList Exception: " + JsonConvert.SerializeObject(ex));

                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        [HttpGet]
        public async Task<ActionResult<HttpResponse<PrintJobReport>>> GetPrintJobReport([FromQuery] string Srno)
        {
            HttpResponse<PrintJobReport> res = new();
            try
            {
                var result = await repo.AllReport.GetPrintJobReport(Srno);
                if (result?.Any() == true)
                {
                    res.ResponseCode = "200";
                    res.ResponseMessage = "Success";
                    res.ResponseData = result;
                    return Ok(res);
                }
                else
                {
                    res.ResponseCode = "404";
                    res.ResponseMessage = "No Data Found";
                    return NotFound(res);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("GetPrintList Request with type: " + Srno);
                _logger.LogError("GetPrintList Exception: " + JsonConvert.SerializeObject(ex));

                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }
    }
}
