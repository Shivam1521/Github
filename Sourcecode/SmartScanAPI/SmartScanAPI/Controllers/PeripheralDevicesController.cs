using Entities;
using IRepository;
using Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace SmartScanAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PeripheralDevicesController(IWrapperRepository repo, ILoggerManager _logger) : ControllerBase
    {
        /// <summary>
        /// To Add or Update Details in Peripheral Device Master
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("CreateUpdatePeripheralDevices")]
        public async Task<ActionResult<Response>> CreateUpdatePeripheralDevices(PeripheralDevicesRequest request)
        {
            Response res = new();
            try
            {
                request.UserId = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);
                res = await repo.PeripheralDevices.CreateUpdatePeripheralDevices(request);
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
                _logger.LogInfo("CreateUpdatePeripheralDevices Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("CreateUpdatePeripheralDevices: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Get List of Peripheral Devices to Bind in Table
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("GetPeripheralDevices")]
        public async Task<ActionResult<HttpResponse<PeripheralDevicesResponse>>> GetPeripheralDevices(TableRequest request)
        {
            HttpResponse<PeripheralDevicesResponse> res = new();
            try
            {
                request.UserId = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);
                var result = await repo.PeripheralDevices.GetPeripheralDevices(request);
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
        /// To Get List of Peripheral Devices for Dropdown.
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetPeripheralDevicesDDL")]
        public async Task<ActionResult<HttpResponse<PeripheralDevicesDDL>>> GetPeripheralDevicesDDL()
        {
            HttpResponse<PeripheralDevicesDDL> res = new();
            try
            {
                var userId = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);
                var result = await repo.PeripheralDevices.GetPeripheralDevicesDDL(userId);
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
                _logger.LogInfo("GetPeripheralDevicesDDL Request: ");
                _logger.LogError("GetPeripheralDevicesDDL: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }
    }
}
