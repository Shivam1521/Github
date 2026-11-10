using IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Entities;
using Newtonsoft.Json;
using Logger;
using System.IdentityModel.Tokens.Jwt;

namespace SmartScanAPI.Controllers
{
    [Authorize(Policy = "SystemOnly")]
    [Route("api/[controller]")]
    [ApiController]
    public class ERPController(IWrapperRepository repo, ILoggerManager _logger) : ControllerBase
    {
        [HttpPost("Items")]
        public async Task<ActionResult<Response>> Items(List<Items> skus)
        {
            _logger.LogInfo("PushSKUs Request: " + JsonConvert.SerializeObject(skus));
            Response res = new();
            try
            {
                if (skus == null || skus.Count == 0)
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = "No SKUs provided.";
                    return BadRequest(res);
                }


                // validate each record
                var invalidRecords = new List<Items>();
                foreach (var sku in skus)
                {
                    if (string.IsNullOrWhiteSpace(sku.ItemCode) ||
                        string.IsNullOrWhiteSpace(sku.Description)) //||
                        //sku.OuterBoxPackingSize == 0)
                    {
                        invalidRecords.Add(sku);
                    }
                }

                if (invalidRecords.Count != 0)
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = "Some records are invalid.";
                    return BadRequest(res);
                    //return BadRequest(new
                    //{
                    //    message = "Some records are invalid.",
                    //    invalidCount = invalidRecords.Count,
                    //    invalidRecords
                    //});
                }
                int userId = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);
                var result = await repo.ERP.MergeSKUs(skus, userId);
                _logger.LogInfo(" PushSKUs Result: " + JsonConvert.SerializeObject(result));
                if (result.ResponseCode == "200")
                {
                    res.ResponseCode = "200";
                    res.ResponseMessage = "Data Added/Updated Successfully";
                    return Ok(res);
                }
                else
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = "Failed to save Add/Update SKUs";
                    return BadRequest(res);
                }
            }
            catch (Exception ex)
            {

                _logger.LogError("PushSKUs: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        [HttpPost("Plants")]
        public async Task<ActionResult<Response>> Plants(List<Plants> plants)
        {
            Response res = new();
            try
            {
                if (plants == null || plants.Count == 0)
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = "No Plant Details provided.";
                    return BadRequest(res);
                }


                // validate each record
                var invalidRecords = new List<Plants>();
                foreach (var plant in plants)
                {
                    if (string.IsNullOrWhiteSpace(plant.PlantCode) ||
                        string.IsNullOrWhiteSpace(plant.PlantName) ||
                        string.IsNullOrWhiteSpace(plant.PlantType) ||
                        string.IsNullOrWhiteSpace(plant.AddressLine) ||
                        string.IsNullOrWhiteSpace(plant.City) ||
                        string.IsNullOrWhiteSpace(plant.State) ||
                        string.IsNullOrWhiteSpace(plant.Country) ||
                        string.IsNullOrWhiteSpace(plant.PostalCode))
                    {
                        invalidRecords.Add(plant);
                    }
                }

                if (invalidRecords.Count != 0)
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = "Some records are invalid.";
                    return BadRequest(res);
                    //return BadRequest(new
                    //{
                    //    message = "Some records are invalid.",
                    //    invalidCount = invalidRecords.Count,
                    //    invalidRecords
                    //});
                }
                int userId = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);
                var result = await repo.ERP.MergePlants(plants, userId);
                if (result.ResponseCode == "200")
                {
                    res.ResponseCode = "200";
                    res.ResponseMessage = "Data Added/Updated Successfully";
                    return Ok(res);
                }
                else
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = "Failed to save Add/Update Plant Details";
                    return BadRequest(res);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("PushPlants Request: " + JsonConvert.SerializeObject(plants));
                _logger.LogError("PushPlants: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        [HttpPost("PickSlips")]
        public async Task<ActionResult<Response>> PickSlips(List<PickSlips> pickSlips)
        {
            Response res = new();
            try
            {
                if (pickSlips == null || pickSlips.Count == 0)
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = "No Pickslip Details provided.";
                    return BadRequest(res);
                }

                // validate each record
                var invalidRecords = new List<PickSlips>();
                foreach (var pickSlip in pickSlips)
                {
                    bool isInvalid = false;

                    // Common mandatory fields
                    if (string.IsNullOrWhiteSpace(pickSlip.PickSlipNumber) ||
                        string.IsNullOrWhiteSpace(pickSlip.ItemCode) ||
                        string.IsNullOrWhiteSpace(pickSlip.SourceBU) ||
                        string.IsNullOrWhiteSpace(pickSlip.DeliveryDtlId) ||
                        string.IsNullOrWhiteSpace(pickSlip.LineStatus))
                    {
                        isInvalid = true;
                    }

                    // ORDER_TYPE based validation
                    if (pickSlip.OrderType == "INTERNAL" &&
                        string.IsNullOrWhiteSpace(pickSlip.DestinationBU))
                    {
                        isInvalid = true;
                    }

                    // EXTERNAL → DestinationBU can be blank (no check needed)

                    // Existing condition
                    if (pickSlip.LineStatus == "Staged/Pick Confirmed" && pickSlip.Quantity <= 0)
                    {
                        isInvalid = true;
                    }

                    if (isInvalid)
                    {
                        invalidRecords.Add(pickSlip);
                    }
                }

                if (invalidRecords.Count != 0)
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = "Some records are invalid.";
                    return BadRequest(res);
                    //return BadRequest(new
                    //{
                    //    message = "Some records are invalid.",
                    //    invalidCount = invalidRecords.Count,
                    //    invalidRecords
                    //});
                }

                int userId = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);
                var result = await repo.ERP.InsertPickSlips(pickSlips, userId);
                if (result.ResponseCode == "200")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("PickSlips Request: " + JsonConvert.SerializeObject(pickSlips));
                _logger.LogError("PickSlips: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }


        [HttpPost("MergePickSlips")]
        public async Task<ActionResult<Response>> MergePickSlips(List<MergePickSlips> pickSlips)
        {
            Response res = new();
            try
            {
                if (pickSlips == null || pickSlips.Count == 0)
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = "No Merge Pickslip Details provided.";
                    return BadRequest(res);
                }

                // validate each record
                var invalidRecords = new List<MergePickSlips>();
                foreach (var pickSlip in pickSlips)
                {
                    bool isInvalid = false;

                    // Common mandatory fields
                    if (string.IsNullOrWhiteSpace(pickSlip.CHILD_PICKSLIP_NUM) ||
                        string.IsNullOrWhiteSpace(pickSlip.PARENT_PICKSLIP_NUM) ||
                        string.IsNullOrWhiteSpace(pickSlip.DeliveryDtlId) ||
                        string.IsNullOrWhiteSpace(pickSlip.UpdatedOn))
                    {
                        isInvalid = true;
                    }

                    if (isInvalid)
                    {
                        invalidRecords.Add(pickSlip);
                    }
                }

                if (invalidRecords.Count != 0)
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = "Some records are invalid.";
                    return BadRequest(res);
                }

                int userId = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);
                var result = await repo.ERP.MergeInsertPickSlips(pickSlips, userId);
                if (result.ResponseCode == "200")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("PickSlips Request: " + JsonConvert.SerializeObject(pickSlips));
                _logger.LogError("PickSlips: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        [HttpPost("Kanban")]
        public async Task<ActionResult<Response>> Kanban(List<Kanban> list)
        {
            Response res = new();
            try
            {
                if (list == null || list.Count == 0)
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = "No Pickslip Details provided.";
                    return BadRequest(res);
                }

                // validate each record
                var invalidRecords = new List<Kanban>();
                foreach (var item in list)
                {
                    if (string.IsNullOrWhiteSpace(item.MfgLocation) ||
                        string.IsNullOrWhiteSpace(item.ItemCode) ||
                        item.LotSize <= 0 ||
                        item.CardQty <= 0 ||
                        item.QtyToBeDispatched <= 0 ||
                        string.IsNullOrWhiteSpace(item.Priority) ||
                        string.IsNullOrWhiteSpace(item.TxnDateTime) ||
                        item.MSL <= 0 ||
                        item.WHOnHand <= 0 ||
                        item.CardPicked <= 0 ||
                        item.PickedQty <= 0)
                    {
                        invalidRecords.Add(item);
                    }
                }

                if (invalidRecords.Count != 0)
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = "Some records are invalid.";
                    return BadRequest(res);
                    //return BadRequest(new
                    //{
                    //    message = "Some records are invalid.",
                    //    invalidCount = invalidRecords.Count,
                    //    invalidRecords
                    //});
                }

                int userId = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);
                var result = await repo.ERP.InsertUpdateKanban(list, userId);
                if (result.ResponseCode == "200")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("Kanban Request: " + JsonConvert.SerializeObject(list));
                _logger.LogError("Kanban: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

    }
}
