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
    public class UserController(ILoggerManager _logger, IWrapperRepository repo) : ControllerBase
    {
        /// <summary>
        /// To Get List of Users to Bind in Table
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("GetUsers")]
        public async Task<ActionResult<HttpResponse<UserMasterReponse>>> GetUsers(TableRequest request)
        {
            HttpResponse<UserMasterReponse> res = new();
            try
            {
                var result = await repo.User.GetAllUsersAsync(request);
                if (result != null && result.Any())
                {
                    res.ResponseCode = "200";
                    res.ResponseMessage = "Success";
                    foreach (var item in result)
                    {
                        if (!string.IsNullOrEmpty(item.Line))
                        {
                            try
                            {
                                item.Lines = JsonConvert.DeserializeObject<List<Line>>(item.Line) ?? new List<Line>();
                            }
                            catch (JsonException)
                            {
                                item.Lines = [];
                            }
                        }
                        else
                        {
                            item.Lines = [];
                        }
                    }
                    res.ResponseData = result;
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
                _logger.LogInfo("GetUsers Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("GetUsers: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Create or Update a User
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("CreateUpdateUser")]
        public async Task<ActionResult<Response>> CreateUpdateUser(UserMasterRequest request)
        {
            Response res = new();
            try
            {
                if (!string.IsNullOrEmpty(request.Password))
                {
                    request.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                }
                res = await repo.User.CreateUpdateUser(request);
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
                _logger.LogInfo("CreateUpdateUser Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("CreateUpdateUser: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Create or Update a End User Mobile
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("CreateEndUser")]
        public async Task<ActionResult<Response>> CreateEndUser(MobileUser request)
        {
            Response res = new();
            try
            {
                if (!string.IsNullOrEmpty(request.Password))
                {
                    request.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                }
                res = await repo.User.CreateEndUser(request);
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
                _logger.LogInfo("CreateUpdateUser Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("CreateUpdateUser: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Update MPIN of the User
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("UpdateMPIN")]
        public async Task<ActionResult<Response>> UpdateMPIN(MPINRequest request)
        {
            Response res = new();
            try
            {
                if (!string.IsNullOrEmpty(request.MPIN))
                {
                    request.MPIN = BCrypt.Net.BCrypt.HashPassword(request.MPIN);
                }
                res = await repo.User.UpdateMPIN(request);
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
                _logger.LogInfo("MPINRequest Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("MPINRequest: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }


        /// <summary>
        /// To Delete Mobile User 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("DeleteEndUserMobile")]
        public async Task<SingleResponse<AuthMobileResponse>> DeleteUserMobile(int UserId)
        {
            var res = new SingleResponse<AuthMobileResponse>();

            try
            {
                var deleteResponse = await repo.User.DeleteUserMobile(UserId);

                if (deleteResponse == null || deleteResponse.ResponseCode != "200")
                {
                    res.ResponseCode = deleteResponse?.ResponseCode ?? "500";
                    res.ResponseMessage = deleteResponse?.ResponseMessage ?? "Failed to delete user";
                    return res;
                }

                res.ResponseCode = "200";
                res.ResponseMessage = "User account deleted successfully";
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogInfo("DeleteUserMobile Request: " + JsonConvert.SerializeObject(new { UserId }));
                _logger.LogError("DeleteUserMobile Exception: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "An internal error occurred. Please try again.";
                return res;
            }
        }


    }
}
