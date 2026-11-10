using Entities;
using IRepository;
using Logger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SmartScanAPI.Helper;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SmartScanAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(ILoggerManager _logger, AuthService authService, IOptions<JwtSettings> settings, CaptchaService captchaService, IWrapperRepository repo) : ControllerBase
    {
        private readonly TokenGenerator _tokenGen = new(settings.Value);
        /// <summary>
        /// To Authenticate User and Generate JWT Token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        public async Task<ActionResult<SingleResponse<AuthResponse>>> Login(AuthRequest request)
        {
            SingleResponse<AuthResponse> res = new();
            try
            {
                var captchaValid = await captchaService.VerifyCaptchaAsync(request.CaptchaToken);
                if (!captchaValid)
                {
                    res.ResponseCode = "401";
                    res.ResponseMessage = "Captcha validation failed";
                    return Unauthorized(res);
                }

                var result = await authService.Authenticate(request);
                if (result != null)
                {
                    res.ResponseCode = "200";
                    res.ResponseMessage = "Success";
                    res.ResponseData = result;
                    return Ok(res);
                }
                else
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = "Invalid Username or Password";
                    return BadRequest(res);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("Login Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("Login Error: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Authenticate Mobile User and Generate JWT Token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("LoginMobile")]
        public async Task<ActionResult<SingleResponse<AuthMobileResponse>>> LoginMobile(AuthRequest request)
        {
            SingleResponse<AuthMobileResponse> res = new();
            try
            {
                var result = await authService.AuthenticateMobile(request);

                if (result != null && result.IsBlocked == false)
                {
                    res.ResponseCode = "200";
                    res.ResponseMessage = "Success";
                    res.ResponseData = result;
                    return Ok(res);
                }
                else if(result != null && result.IsBlocked == true)
                {
                    res.ResponseCode = "403";
                    res.ResponseMessage = "403 - User is blocked..";
                    return BadRequest(res);
                }
                else
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = "Invalid Username or Password";
                    return BadRequest(res);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("Login Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("Login Error: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Generate Fresh token based on provided access token and refresh token
        /// </summary>
        /// <param name="tokenApiprintingDetailsRequest"></param>
        /// <returns></returns>
        [HttpPost("Refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenRequest request)
        {
            var principal = _tokenGen.GetPrincipalFromExpiredToken(request.AccessToken);

            var userId = int.Parse(principal.FindFirst(JwtRegisteredClaimNames.Sid)!.Value);
            var email = principal.FindFirst(ClaimTypes.Email)!.Value;
            var roleId = int.Parse(principal.FindFirst(ClaimTypes.Role)!.Value);
            var tokenVersion = int.Parse(principal.FindFirst("tv")!.Value);

            var user = await repo.User.GetUserDetails(userId);

            if (user?.TokenVersion != tokenVersion)
                return Unauthorized("Invalid or expired refresh token");

            var result = await authService.RefreshTokens(
                userId,
                request.RefreshToken,
                email,
                roleId,
                tokenVersion
            );

            if (result == null)
                return Unauthorized("Invalid or expired refresh token");

            return Ok(new SingleResponse<AuthenticatedResponse>
            {
                ResponseCode = "200",
                ResponseMessage = "Token refreshed successfully",
                ResponseData = result
            });
        }


        [HttpPost("Token")]
        public async Task<IActionResult> GetToken([FromBody] AuthRequest request)
        {
            try
            {
                var token = await authService.Authenticate(request);
                if (token == null) return Unauthorized();
                return Ok(new { token.Token });
            }
            catch (Exception ex)
            {
                _logger.LogInfo("GetToken Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("GetToken Error: " + JsonConvert.SerializeObject(ex));
                return StatusCode(500, "Internal Server Error, Please Try again later or contact admin");
            }
        }


        [HttpPost("AccountDeleteUserMobile")]
        public async Task<SingleResponse<AuthMobileResponse>> AccountDeleteUserMobile(AuthRequest user)
        {
            SingleResponse<AuthMobileResponse> res = new SingleResponse<AuthMobileResponse>();
            try
            {
                var authResult = await authService.Authenticate(user);

                if (authResult == null)
                {
                    res.ResponseCode = "404";
                    res.ResponseMessage = "User not found";
                    res.ResponseData = null;
                    return res;
                }

                if (Convert.ToInt32(authResult.UserType) != 2)
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = "Access Denied - Mobile User";
                    res.ResponseData = null;
                    return res;
                }
               
            

               

                var deleteResponse = await repo.User.DeleteUserWeb(authResult.Email);

                if (deleteResponse == null || deleteResponse.ResponseCode != "200")
                {
                    return new SingleResponse<AuthMobileResponse>
                    {
                        ResponseCode = deleteResponse?.ResponseCode ?? "500",
                        ResponseMessage = deleteResponse?.ResponseMessage ?? "Failed to delete user"
                    };
                }

                return new SingleResponse<AuthMobileResponse>
                {
                    ResponseCode = "200",
                    ResponseMessage = "User account deleted successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                res.ResponseCode = "500";
                res.ResponseMessage = "An internal error occurred. Please try again.";
            }

            return res;
        }

    }
}
