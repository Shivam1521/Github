using Entities;
using IRepository;
using Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartScanAPI.Helper;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace SmartScanAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class QCController(IWrapperRepository repo, ILoggerManager _logger, AppSecrets secrets, EnvironmentVariables variables) : ControllerBase
    {
        private readonly QRGenerator qrGenerator = new(secrets, variables);

        /// <summary>
        /// To Get Carton Details by QR Code
        /// </summary>
        /// <param name="qr"></param>
        /// <returns></returns>
        [HttpGet("GetCartonDetails")]
        public async Task<ActionResult<SingleResponse<CartonDetails>>> GetCartonDetails(string qr, int actionType)
        {
            SingleResponse<CartonDetails> res = new();
            try
            {
                var parsedQR = ParseQR(qr);
                _logger.LogInfo("GetCartonDetails Parsed QR: " + JsonConvert.SerializeObject(parsedQR));
                if (parsedQR != null && parsedQR.SeqNo != -1)
                {
                    if (actionType == 2 && parsedQR.QRType != PassPhrases.PrePrintedSecondary)
                    {
                        res.ResponseCode = "400";
                        res.ResponseMessage = $"Invalid QR Code. Please Scan Preprinted Outer QR Code";
                        return BadRequest(res);
                    }

                    var cartonDetails = await repo.QC.GetCartonDetails(parsedQR.Prefix, parsedQR.SeqNo, parsedQR.QRType, actionType);
                    if (cartonDetails.SeqNo != 0 && cartonDetails.SeqNo > 0)
                    {
                        res.ResponseCode = "200";
                        res.ResponseMessage = "Success";
                        res.ResponseData = cartonDetails;
                        return Ok(res);
                    }
                    else
                    {
                        res.ResponseCode = "404";
                        if (cartonDetails.SeqNo == -1)
                        {
                            res.ResponseMessage = "This Carton QR is not printed yet";
                        }
                        else if (cartonDetails.SeqNo == -2)
                        {
                            res.ResponseMessage = "Scanned QR is already replaced";
                        }
                        else if (cartonDetails.SeqNo == -3)
                        {
                            res.ResponseMessage = "Scanned QR is already used for replacement";
                        }
                        else if (cartonDetails.SeqNo == -4)
                        {
                            res.ResponseMessage = "Scanned QR Code should be an Outer QR";
                        }
                        else
                        {
                            res.ResponseMessage = "Carton details not found.";
                        }
                        return NotFound(res);
                    }
                }
                else
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = $"Invalid QR Code. {(parsedQR?.Reason ?? "Unknown error")}";
                    return BadRequest(res);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("GetCartonDetails Request: " + qr + "&AT: " + actionType);
                _logger.LogError("GetCartonDetails: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To replace outer QR with preprinted outer QR at the time of OQC/PQC
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("ReplaceOuterForQC")]
        public async Task<ActionResult<Response>> ReplaceOuterForQC(ReplaceCartonRequest request)
        {
            Response res = new();
            try
            {
                request.PerformedBy = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);
                res = await repo.QC.ReplaceOuterForQC(request);
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
                _logger.LogInfo("ReplaceOuterForQC Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("ReplaceOuterForQC: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Verify QR Code Status for QC Start
        /// </summary>
        /// <param name="qr"></param>
        /// <returns></returns>
        [HttpGet("GetQCStatus")]
        public async Task<ActionResult<SingleResponse<QCStatusDetails>>> GetQCStatus(string qr)
        {
            SingleResponse<QCStatusDetails> res = new();
            try
            {
                var parsedQR = ParseQR(qr);
                _logger.LogInfo("GetQCStatus Parsed QR: " + JsonConvert.SerializeObject(parsedQR));
                if (parsedQR != null && parsedQR.SeqNo != -1)
                {
                    var cartonDetails = await repo.QC.GetQCStatus(parsedQR.Prefix, parsedQR.SeqNo, parsedQR.QRType);
                    if (cartonDetails.ResponseCode == "200")
                    {
                        res.ResponseCode = "200";
                        res.ResponseMessage = "Success";
                        res.ResponseData = cartonDetails;
                        return Ok(res);
                    }
                    else
                    {
                        res.ResponseCode = cartonDetails.ResponseCode;
                        res.ResponseMessage = cartonDetails.ResponseMessage;
                        return BadRequest(res);
                    }
                }
                else
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = $"Invalid QR Code. {(parsedQR?.Reason ?? "Unknown error")}";
                    return BadRequest(res);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("GetQCStatus Request: " + qr);
                _logger.LogError("GetQCStatus: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To mark QC as Open (QCStatus = 1) for provided QRs
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("MarkQCOpen")]
        public async Task<ActionResult<Response>> MarkQCOpen(MarkQCOpenRequest request)
        {
            Response res = new();
            try
            {
                request.PerformedBy = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);
                res = await repo.QC.MarkQCOpen(request);
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
                _logger.LogInfo("MarkQCOpen Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("MarkQCOpen: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Get Job details for QC
        /// </summary>
        /// <param name="qr"></param>
        /// <returns></returns>
        [HttpGet("GetJobDetailsForQC")]
        public async Task<ActionResult<SingleResponse<JobDetailsForQC>>> GetJobDetailsForQC(string qr)
        {
            SingleResponse<JobDetailsForQC> res = new();
            try
            {
                var parsedQR = ParseQR(qr);
                _logger.LogInfo("GetJobDetailsForQC Parsed QR: " + JsonConvert.SerializeObject(parsedQR));
                if (parsedQR != null && parsedQR.SeqNo != -1)
                {
                    var cartonDetails = await repo.QC.GetJobDetailsForQC(parsedQR.Prefix, parsedQR.SeqNo, parsedQR.QRType);
                    if (cartonDetails.ResponseCode == "200")
                    {
                        res.ResponseCode = "200";
                        res.ResponseMessage = "Success";
                        res.ResponseData = cartonDetails;
                        return Ok(res);
                    }
                    else
                    {
                        res.ResponseCode = cartonDetails.ResponseCode;
                        res.ResponseMessage = cartonDetails.ResponseMessage;
                        return BadRequest(res);
                    }
                }
                else
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = $"Invalid QR Code. {(parsedQR?.Reason ?? "Unknown error")}";
                    return BadRequest(res);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("GetJobDetailsForQC Request: " + qr);
                _logger.LogError("GetJobDetailsForQC: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// Update QCStatus (2=Pass, 3=Hold, 4=Reject) for all linked records of a given JobId
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("UpdateQCStatus")]
        public async Task<ActionResult<Response>> UpdateQCStatus(UpdateQCStatusRequest request)
        {
            Response res = new();
            try
            {
                request.PerformedBy = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);
                res = await repo.QC.UpdateQCStatus(request);
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
                _logger.LogInfo("UpdateQCStatus Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("UpdateQCStatus: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To Get Carton Details by QR Code
        /// </summary>
        /// <param name="qr"></param>
        /// <returns></returns>
        [HttpGet("GetCartonDetailsForQC")]
        public async Task<ActionResult<SingleResponse<CartonDetailsForQC>>> GetCartonDetailsForQC(string qr, int actionType)
        {
            SingleResponse<CartonDetailsForQC> res = new();
            try
            {
                var parsedQR = ParseQR(qr);
                _logger.LogInfo("GetCartonDetailsForQC Parsed QR: " + JsonConvert.SerializeObject(parsedQR));
                if (parsedQR != null && parsedQR.SeqNo != -1)
                {
                    if (actionType == 2 && parsedQR.QRType != PassPhrases.PrePrintedSecondary)
                    {
                        res.ResponseCode = "400";
                        res.ResponseMessage = $"Invalid QR Code. Please Scan Preprinted Outer QR Code";
                        return BadRequest(res);
                    }

                    var cartonDetails = await repo.QC.GetCartonDetailsForQC(parsedQR.Prefix, parsedQR.SeqNo, parsedQR.QRType, actionType);
                    if (cartonDetails.SeqNo != 0 && cartonDetails.SeqNo > 0)
                    {
                        res.ResponseCode = cartonDetails.ResponseCode;
                        res.ResponseMessage = cartonDetails.ResponseMessage;
                        res.ResponseData = cartonDetails;
                        return Ok(res);
                    }
                    else
                    {
                        res.ResponseCode = cartonDetails.ResponseCode;
                        res.ResponseMessage = cartonDetails.ResponseMessage;
                        return NotFound(res);
                    }
                }
                else
                {
                    res.ResponseCode = "400";
                    res.ResponseMessage = $"Invalid QR Code. {(parsedQR?.Reason ?? "Unknown error")}";
                    return BadRequest(res);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("GetCartonDetailsForQC Request: " + qr + "&AT: " + actionType);
                _logger.LogError("GetCartonDetailsForQC: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// To replace outer QR with preprinted outer QR at the time of OQC/PQC
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("ReplaceOuterAfterQC")]
        public async Task<ActionResult<Response>> ReplaceOuterAfterQC(ReplaceOuterAfterQCRequest request)
        {
            Response res = new();
            try
            {
                request.PerformedBy = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);
                res = await repo.QC.ReplaceOuterAfterQC(request);
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
                _logger.LogInfo("ReplaceOuterAfterQC Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("ReplaceOuterAfterQC: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }

        /// <summary>
        /// Parses and decrypts a QR string.
        /// </summary>
        private QRResult ParseQR(string qrData)
        {
            QRResult carton = new()
            {
                SeqNo = -1, // default invalid
                Prefix = string.Empty,
                QRType = "U",
                Reason = string.Empty
            };

            if (string.IsNullOrWhiteSpace(qrData))
            {
                carton.Reason = "QRData is null or empty.";
                _logger.LogError("QRData is null or empty.");
                return carton;
            }

            try
            {
                // Fix encoding issue
                qrData = qrData.Replace(":%5D", ":]");

                // Validate and extract encrypted part
                var parts = qrData.Split("pa?uid=:]C1");
                if (parts.Length < 2 || string.IsNullOrWhiteSpace(parts[1]))
                {
                    carton.Reason = "Invalid QR format: missing expected delimiter.";
                    _logger.LogError($"Invalid QR format: missing expected delimiter in {qrData}");
                    return carton;
                }

                var encryptedSegment = parts[1].Replace(":", "");
                var decrypted = qrGenerator.Decrypt(encryptedSegment);

                if (string.IsNullOrWhiteSpace(decrypted) || decrypted.Length < 4)
                {
                    carton.Reason = $"Invalid decrypted data for QR: {qrData}";
                    _logger.LogError($"Invalid decrypted data for QR: {qrData}");
                    return carton;
                }

                // Extract prefix and sequence
                char qrPrefixChar = decrypted[0];
                carton.Prefix = decrypted.Substring(1, 2);

                if (!long.TryParse(decrypted[3..], out long seqNo))
                {
                    carton.Reason = $"Failed to parse sequence number in QR: {qrData}";
                    _logger.LogError($"Failed to parse sequence number in QR: {qrData}");
                    return carton;
                }

                carton.SeqNo = seqNo;

                // Map QR type
                carton.QRType = qrPrefixChar switch
                {
                    var c when c.ToString() == PassPhrases.SecondaryPacking => PassPhrases.SecondaryPacking,
                    var c when c.ToString() == PassPhrases.TertiaryPacking => PassPhrases.TertiaryPacking,
                    var c when c.ToString() == PassPhrases.PrePrintedSecondary => PassPhrases.PrePrintedSecondary,
                    var c when c.ToString() == PassPhrases.PrePrintedTertiary => PassPhrases.PrePrintedTertiary,
                    _ => "U" // Unknown
                };
            }
            catch (CryptographicException ex)
            {
                carton.Reason = $"Decryption failed for QR: {qrData}";
                _logger.LogError($"Decryption failed for QR: {qrData} | {JsonConvert.SerializeObject(ex)}");
            }
            catch (Exception ex)
            {
                carton.Reason = $"Error parsing QR: {qrData}";
                _logger.LogError($"Error parsing QR: {qrData} | {JsonConvert.SerializeObject(ex)}");
            }
            return carton;
        }

        /// <summary>
        /// To attach/detach cartons.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("AttachDetachCartons")]
        public async Task<ActionResult<Response>> AttachDetachCartons(ReplaceCartonRequest request)
        {
            Response res = new();
            try
            {
                request.PerformedBy = Convert.ToInt32(User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value);
                res = await repo.QC.AttachDetachCartons(request);
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
                _logger.LogInfo("ReplaceOuterForQC Request: " + JsonConvert.SerializeObject(request));
                _logger.LogError("ReplaceOuterForQC: " + JsonConvert.SerializeObject(ex));
                res.ResponseCode = "500";
                res.ResponseMessage = "Internal Server Error";
                return StatusCode(500, res);
            }
        }
    }
}
