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
    public class RoleController(ILoggerManager _logger, IWrapperRepository repo) : ControllerBase
    {
        /// <summary>
        /// To Get List of Roles to Bind in Table
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("GetRoles")]
        public async Task<ActionResult<HttpResponse<RoleMasterReponse>>> GetRoles(TableRequest request)
        {
            HttpResponse<RoleMasterReponse> res = new();
            try
            {
                var result = await repo.Role.GetAllRolesAsync(request);
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
                _logger.LogInfo("GetRoles Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("GetRoles: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Create or Update a Role
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("CreateUpdateRole")]
        public async Task<ActionResult<Response>> CreateUpdateRole(RoleMaster request)
        {
            Response res = new();
            try
            {
                res = await repo.Role.CreateUpdateRole(request);
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
                _logger.LogInfo("CreateUpdateRole Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("CreateUpdateRole: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Get Pages for a Role to Modify Permissions
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet("GetPages")]
        public async Task<ActionResult<HttpResponse<AssignedGroups>>> GetPages(int roleId)
        {
            HttpResponse<AssignedGroups> res = new();
            try
            {
                var result = await repo.Role.GetPages(roleId);
                if (result != null && result.Count != 0)
                {
                    foreach (var group in result)
                    {
                        group.Children = JsonConvert.DeserializeObject<IEnumerable<AssignedPages>>(group.ChildrenJSON);
                    }
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
                _logger.LogInfo("GetPages Request: " + roleId);
                _logger.LogError("GetPages: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Assign Page Permissions to specified Role
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("AssignPermissions")]
        public async Task<ActionResult<Response>> AssignPermissions(List<PagePermissionRequest> request)
        {
            Response res = new();
            try
            {
                var result = await repo.Role.AssignPermissions(request);
                if (result != null)
                {
                    res.ResponseCode = "200";
                    res.ResponseMessage = result.ResponseMessage;
                    return Ok(res);
                }
                else
                {
                    res.ResponseCode = "404";
                    res.ResponseMessage = "No Permissions found to Assign";
                    return NotFound(res);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("AssignPermissions Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("AssignPermissions: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Get List of Roles based on User Type for Dropdown.
        /// </summary>
        /// <param name="userType"></param>
        /// <returns></returns>
        [HttpGet("GetRolesDDL")]
        public async Task<ActionResult<HttpResponse<DDL>>> GetRolesDDL(int userType)
        {
            HttpResponse<DDL> res = new();
            try
            {
                var result = await repo.Role.GetRolesDDL(userType);
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
                _logger.LogInfo("GetRolesDDL Request: " + userType);
                _logger.LogError("GetRolesDDL: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }
    }
}
