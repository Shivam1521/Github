using Entities;
using IRepository;
using Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace SmartScanAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UnitController(ILoggerManager _logger, IWrapperRepository repo) : ControllerBase
    {
        /// <summary>
        /// To Create or Update a Unit
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("CreateUpdateUnit")]
        public async Task<ActionResult<Response>> CreateUpdateUnit(UnitMasterRequest request)
        {
            Response res = new();
            try
            {
                res = await repo.Unit.CreateUpdateUnit(request);
                if (res.ResponseCode == "200")
                {
                    return Ok(res);
                }
                else
                {
                    return BadRequest(res);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("CreateUpdateUnit Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("CreateUpdateUnit: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Get List of Units to Bind in Table
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("GetUnits")]
        public async Task<ActionResult<HttpResponse<UnitMasterResponse>>> GetUnits(TableRequest request)
        {
            HttpResponse<UnitMasterResponse> res = new();
            try
            {
                var result = await repo.Unit.GetUnits(request);
                if (result != null && result.Any())
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
                _logger.LogInfo("GetUnits Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("GetUnits: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Get List of Units for Dropdown.
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetUnitsDDL")]
        public async Task<ActionResult<HttpResponse<DDL>>> GetUnitsDDL()
        {
            HttpResponse<DDL> res = new();
            try
            {
                var result = await repo.Unit.GetUnitsDDL();
                if (result != null && result.Any())
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
                _logger.LogInfo("GetUnitsDDL Request: ");
                _logger.LogError("GetUnitsDDL: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }
    }
}
