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
    public class PlantWarehouseController(ILoggerManager _logger, IWrapperRepository repo) : ControllerBase
    {
        /// <summary>
        /// To Create or Update a Plant/Warehouse Master Details
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("CreateUpdatePlantWarehouse")]
        public async Task<ActionResult<Response>> CreateUpdatePlantWarehouse(PlantWarehouseRequest request)
        {
            Response res = new();
            try
            {
                res = await repo.PlantWarehouse.CreateUpdatePlantWarehouse(request);
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
                _logger.LogInfo("CreateUpdatePlantWarehouse Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("CreateUpdatePlantWarehouse: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Get List of Plant/Warehouse to Bind in Table
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("GetPlantWarehouses")]
        public async Task<ActionResult<HttpResponse<PlantWarehouseResponse>>> GetPlantWarehouses(TableRequest request)
        {
            HttpResponse<PlantWarehouseResponse> res = new();
            try
            {
                var result = await repo.PlantWarehouse.GetPlantWarehouses(request);
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
                _logger.LogInfo("GetPlantWarehouses Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("GetPlantWarehouses: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Get List of Plant/Warehouse for Dropdown.
        /// </summary>
        /// <param name="userType"></param>
        /// <returns></returns>
        [HttpGet("GetPlantWarehouseDDL")]
        public async Task<ActionResult<HttpResponse<DDL>>> GetPlantWarehouseDDL(int unitId)
        {
            HttpResponse<DDL> res = new();
            try
            {
                var result = await repo.PlantWarehouse.GetPlantWarehouseDDL(unitId);
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
                _logger.LogInfo("GetPlantWarehouseDDL Request: ");
                _logger.LogError("GetPlantWarehouseDDL: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }
    }
}
