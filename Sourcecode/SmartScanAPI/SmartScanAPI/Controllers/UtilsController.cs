using Entities;
using IRepository;
using Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartScanAPI.Helper;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace SmartScanAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UtilsController(ILoggerManager _logger, IWrapperRepository repo, AppSecrets secrets, EnvironmentVariables variables, IWebHostEnvironment _env) : ControllerBase
    {
        private readonly QRGenerator qrGenerator = new(secrets, variables);

        /// <summary>
        /// To Get Country, State, City based on Pincode entered.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("GetPincodeDetails")]
        public async Task<ActionResult<PincodeDetails>> GetPincodeDetails(string pinCode)
        {
            SingleResponse<PincodeDetails> res = new();
            try
            {
                var result = await repo.Utils.GetPincodeDetails(pinCode);
                if (result != null)
                {
                    res.ResponseCode = "200";
                    res.ResponseMessage = "Success";
                    res.ResponseData = result;
                    return Ok(result);
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
                _logger.LogInfo("GetPincodeDetails Request: " + pinCode);
                _logger.LogError("GetPincodeDetails: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        [HttpGet("GetQRInfo")]
        public async Task<ActionResult<SingleResponse<QRParseResult>>> GetQRInfo(string qr)
        {
            try
            {
                SingleResponse<QRParseResult> res = new();
                var parsedQR = CommonMethods.ParseQR(qr, _logger, qrGenerator, "GetOuterQRs");
                res.ResponseCode = "200";
                res.ResponseMessage = "Success.";
                res.ResponseData = parsedQR;
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogInfo("GetOuterQRs Request: " + qr);
                _logger.LogError("GetOuterQRs Error: " + JsonConvert.SerializeObject(ex));
                return StatusCode(500, "Internal Server Error, Please Try again later or contact admin");
            }
        }

        /// <summary>
        /// To Generate Production Line QRs for Testing Purpose or to share with WMS
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("GeneratePLQRs")]
        public async Task<ActionResult<ResponseWithData>> GeneratePLQRs(ProductionLineQRsRequest request)
        {
            ResponseWithData res = new();
            try
            {
                request.UserId = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);

                var lastPrintedNumberDetails = await repo.PrintJob.GeneratePLQRs(request);

                if (lastPrintedNumberDetails == null)
                {
                    res.ResponseCode = "404";
                    res.ResponseMessage = "No Print Data Found for this Job.";
                    return NotFound(res);
                }
                else
                {
                    List<PLQR> pLQRs = [];
                    foreach (var item in lastPrintedNumberDetails)
                    {
                        PLQR qR = new()
                        {
                            ItemCode = item.ItemCode,
                            MasterQR = qrGenerator.GenerateTertiaryQR(item.MasterSeqNo, item.Prefix)
                        };
                        pLQRs.Add(qR);
                    }

                    // In your controller:
                    string webRootPath = _env.WebRootPath; // Inject IWebHostEnvironment _env
                    string folderPath = Path.Combine(webRootPath, "PrintedQRs");
                    Directory.CreateDirectory(folderPath);

                    string fileName = $"{request.ItemCode}_{request.Quantity}_{DateTime.Now:yyyyMMddHHmmssffff}.csv";
                    string filePath = Path.Combine(folderPath, fileName);

                    CommonMethods.ToCsv(pLQRs, filePath);

                    res.ResponseCode = "200";
                    res.ResponseMessage = "Success";
                    res.ResponseData = $"{Request.Scheme}://{Request.Host}/PrintedQRs/{fileName}";
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("GeneratePLQRs Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("GeneratePLQRs: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }
    }
}
