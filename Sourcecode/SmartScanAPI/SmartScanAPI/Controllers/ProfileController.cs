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
    public class ProfileController(IWrapperRepository repo, ILoggerManager _logger) : ControllerBase
    {
        /// <summary>
        /// To Update User Password
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("ChangePassword")]
        public async Task<ActionResult<Response>> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            Response res = new();
            try
            {
                var user = await repo.User.GetUserDetails(request.UserId);

                if (user == null || user.UserId == 0)
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = "User not found";
                    return NotFound(res);
                }

                // Verify old password
                if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash))
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = "Old password is incorrect";
                    return BadRequest(res);
                }

                // Prevent same new password as old password
                if (BCrypt.Net.BCrypt.Verify(request.NewPassword, user.PasswordHash))
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = "New password cannot be the same as the old password";
                    return BadRequest(res);
                }

                // Hash new password
                string newHashedPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

                res = await repo.User.UpdatePasswordAsync(user.UserId, newHashedPassword);

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
                _logger.LogInfo("ChangePassword Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("ChangePassword Error: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }
    }
}
