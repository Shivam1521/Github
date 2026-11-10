using Entities;
using IRepository;
using Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartScanAPI.Helper;

namespace SmartScanAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TrackingController(IWrapperRepository repo, ILoggerManager _logger, AppSecrets secrets, EnvironmentVariables variables) : ControllerBase
    {
        private readonly QRGenerator qrGenerator = new(secrets, variables);

        [HttpGet("GetCartonTrace")]
        public async Task<ActionResult<HttpResponse<CartonTraceResponse>>> GetCartonTrace(string qr)
        {
            HttpResponse<CartonTraceResponse> res = new();
            try
            {
                var parsedQR = qr.Length == 14
                     ? CommonMethods.ParseQR(qr, _logger, qrGenerator, "VerifyQRForPickslip")
                     : CommonMethods.ParseSeqQR(qr, _logger, qrGenerator, "VerifyQRForPickslip");
               
                //var parsedQR = CommonMethods.ParseQR(qr, _logger, qrGenerator, "VerifyQRForPickslip");
                _logger.LogInfo("GetCartonDetailsForQC Parsed QR: " + JsonConvert.SerializeObject(parsedQR));
                if (parsedQR != null && parsedQR.SeqNo != -1)
                {
                    var traceData = await repo.Tracking.GetCartonTrace(parsedQR.SeqNo, parsedQR.Prefix, parsedQR.QRType);
                    if (traceData.Any())
                    {
                        res.ResponseCode = "200";
                        res.ResponseMessage = "Success";
                        res.ResponseData = traceData;
                        return Ok(res);
                    }
                    else
                    {
                        res.ResponseCode = "404";
                        res.ResponseMessage = "No Trace Data Found";
                        return NotFound(res);
                    }
                }
                else
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = $"Invalid QR Code. {(parsedQR?.ParseStatus ?? "Unknown error")}";
                    return BadRequest(res);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("GetCartonTrace Request: " + qr);
                _logger.LogError("GetCartonTrace Error: " + JsonConvert.SerializeObject(ex));
                return StatusCode(500, "Internal Server Error, Please Try again later or contact admin");
            }
        }
    }
}
