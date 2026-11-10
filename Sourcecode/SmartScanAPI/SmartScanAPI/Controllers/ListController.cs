using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
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
    public class ListController(ILoggerManager _logger, IWrapperRepository repo) : ControllerBase
    {
        /// <summary>
        /// To Get List Data based on Type to use at multiple places like Dropdown, Grid etc.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("GetList")]
        public async Task<ActionResult<HttpResponse<DDL>>> GetList(string listName, int? unitId)
        {
            HttpResponse<DDL> res = new();
            try
            {
                int userId = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);
                var result = await repo.List.GetList(listName, unitId);
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
                _logger.LogInfo("GetList Request: " + listName);
                _logger.LogError("GetList: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Get List Headers for binding in Dropdown
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("GetListHeaders")]
        public async Task<ActionResult<HttpResponse<ListMaster>>> GetListHeaders()
        {
            HttpResponse<ListMaster> res = new();
            try
            {
                var result = await repo.List.GetListHeaders();
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
                _logger.LogInfo("GetListHeaders Request: ");
                _logger.LogError("GetListHeaders: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Get List Details Data for binding Grid
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("GetListDetails")]
        public async Task<ActionResult<HttpResponse<ListDetailResponse>>> GetListDetails(TableRequest request)
        {
            HttpResponse<ListDetailResponse> res = new();
            try
            {
                request.UserId = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);
                var result = await repo.List.GetListDetails(request);
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
                _logger.LogInfo("GetListDetails Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("GetListDetails: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Add or Update a record in List Master
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        [HttpPost("AddUpdateListMaster")]
        public async Task<ActionResult<Response>> AddUpdateListMaster(ListDetail detail)
        {
            Response res = new();
            try
            {
                detail.UserId = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);
                res = await repo.List.AddUpdateListMaster(detail);
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
                _logger.LogInfo("AddUpdateListMaster Request: " + JsonConvert.SerializeObject(detail));
                _logger.LogError("AddUpdateListMaster: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }
    }
}
