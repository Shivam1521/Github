using Entities;
using IRepository;
using Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace SmartScanAPI.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    [Authorize]
    public class PrintingController(ILoggerManager _logger, IWrapperRepository repo) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<SingleResponse<bool>>> SavePrintingDetails([FromForm] List<PrintingDetailsRequest> printingDetailsRequest)
        {
            SingleResponse<bool> res = new();
            try
            {
                var result = await repo.PrintingDetails.SavePrintingDetailsAsync(printingDetailsRequest);
                if (result)
                {
                    res.ResponseCode = "200";
                    res.ResponseMessage = "Printing Data save Success";
                    res.ResponseData = true;
                    return Ok(res);
                }
                else
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = "Failed to save printing details";
                    return BadRequest(res);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("SavePrintingDetails Request: " + JsonConvert.SerializeObject(printingDetailsRequest));
                _logger.LogError("SavePrintingDetails: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        [HttpPost]
        public async Task<ActionResult<SingleResponse<int>>> SavePrintedData(SavePrintedDataRequest request)
        {
            SingleResponse<int> res = new();
            try
            {
                var result = await repo.PrintingDetails.SavePrintedData(request);
                if (result == 1)
                {
                    res.ResponseCode = "200";
                    res.ResponseMessage = "Printing Data Added Success";
                    res.ResponseData = 1;
                    return Ok(res);
                }
                else if (result == 2)
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = "Printing Data Already Exists";
                    return BadRequest(res);
                }
                else
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = "Failed to save printing details";
                    return BadRequest(res);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("SavePrintedData Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("SavePrintedData: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        [HttpGet]
        public async Task<ActionResult<HttpResponse<PrintingDetailsResponse>>> GetPrintingDetails()
        {
            HttpResponse<PrintingDetailsResponse> res = new();
            try
            {
                var getPrintingDetails = await repo.PrintingDetails.GetPrintingDetails();
                if (getPrintingDetails.Count() > 0)
                {
                    res.ResponseCode = "200";
                    res.ResponseMessage = "Success";
                    res.ResponseData = getPrintingDetails;
                    return Ok(res);
                }
                else
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = "Invalid Username or Password";
                    return NotFound(res);
                }
            }
            catch (Exception ex)
            {

                _logger.LogInfo("GetUsers Request: " + JsonConvert.SerializeObject(res.ResponseData));
                _logger.LogError("GetUsers: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        [HttpPost]
        public async Task<ActionResult<SingleResponse<int>>> SaveMasterPrinting(MasterPrintingRequest request)
        {
            SingleResponse<int> res = new();
            try
            {
                var result = await repo.PrintingDetails.SaveMasterPrinting(request);
                if (result == 1)
                {
                    res.ResponseCode = "200";
                    res.ResponseMessage = "Printing Data Added Success";
                    res.ResponseData = 1;
                    return Ok(res);
                }
                else if (result == 2)
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = "Printing Data Already Exists";
                    return BadRequest(res);
                }
                else if (result == 3)
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = "Secondery packing not  Exists";
                    return BadRequest(res);
                }
                else
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = "Failed to save printing details";
                    return BadRequest(res);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("SavePrintedData Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("SavePrintedData: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        [HttpGet]
        public async Task<ActionResult<HttpResponse<PrintList>>> GetPrintList([FromQuery] string type)
        {
            HttpResponse<PrintList> res = new();

            try
            {
                int createdBy = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);
                var result = await repo.PrintingDetails.GetPrintList(type, createdBy);

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
                _logger.LogInfo("GetPrintList Request with type: " + type);
                _logger.LogError("GetPrintList Exception: " + JsonConvert.SerializeObject(ex));

                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }
    }
}
